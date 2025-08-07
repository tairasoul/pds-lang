local function parseParams(params)
  local use_count = false
  local count = 1
  for _,v in pairs(params) do
    if v == "use_count" then use_count = true goto continue end
    if string.match(v, "count=") then
      count = tonumber(_gsub(v, "count=", ""))
    end
    ::continue::
  end
  return use_count, count
end

local function parseFilters(filters, x, y)
  local processed = {}
  local index = 0
  for _,v in pairs(filters) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,res in pairs(result) do
        table.insert(processed, res.baseTable)
      end
    end
  end
  return processed
end

local function parseCrafting(x, y, params, line1, line2, line3)
  local uc, c = parseParams(params)
  local widget = {
    name = "pneumaticcraft:crafting",
    x = x,
    y = y,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 33,
    use_count = uc,
    count = c
  }
  local l1 = parseFilters(line1, x, y)
  local l2 = parseFilters(line2, x, y + 11)
  local l3 = parseFilters(line3, x, y + 22)
  local ret = {
    widget
  }
  if #l1 > 0 then
    for _,v in pairs(l1) do table.insert(ret, v) end
  end
  if #l2 > 0 then
    for _,v in pairs(l2) do table.insert(ret, v) end
  end
  if #l3 > 0 then
    for _,v in pairs(l3) do table.insert(ret, v) end
  end
  return ret
end

return {
  identifier = "craft",
  processor = parseCrafting,
  arguments = {
    {
      name = "params",
      types = { "string[]" }
    },
    {
      name = "line1",
      types = { "item_filter[]" }
    },
    {
      name = "line2",
      types = { "item_filter[]" }
    },
    {
      name = "line3",
      types = { "item_filter[]" }
    }
  }
}