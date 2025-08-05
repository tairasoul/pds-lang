local function processFilter(filter)
  local chkComponents = false
  local chkMod = false
  local chkBlock = false
  local chkDurability = false
  for _,v in pairs(filter) do
    if v == "components" then chkComponents = true end
    if v == "mod" then chkMod = true end
    if v == "block" then chkBlock = true end
    if v == "durability" then chkDurability = true end
  end
  return chkComponents, chkMod, chkBlock, chkDurability
end

local function split(inputstr, sep)
  if sep == nil then
    sep = "%s"
  end
  local t = {}
  for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
    table.insert(t, str)
  end
  return t
end


local function processItemFilter(x, y, item, filter, global)
  local comp, mod, block, durab = processFilter(filter)
  local widget = {
    name = "pneumaticcraft:item_filter",
    x = x,
    y = y,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
    chk_durability = durab,
    chk_block = block,
    chk_mod = mod,
    chk_components = comp
  }
  if global then
    widget.var = item
  else
    local splitted = split(item, "&")
    local itm = splitted[1]
    local count = tonumber(splitted[2] or "1")
    widget.chk_item = {
      id = itm,
      count = count
    }
  end
  return {
    widget
  }
end

return {
  identifier = "item_filter",
  processor = processItemFilter,
  arguments = {
    {
      name = "item",
      types = { "string" }
    },
    {
      name = "filter",
      types = { "table" }
    },
    {
      name = "global",
      types = { "bool" },
      required = false
    }
  },
  validOutsideArguments = false
}