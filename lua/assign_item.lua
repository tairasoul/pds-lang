local function parseItemAssignment(x, y, var, filter)
  local widget = {
    name = "pneumaticcraft:item_assign",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    var = var
  }
  local ret = { widget }
  if filter ~= nil then
    local parser = filter.parser
    local args = filter.objects
    if parser:validateArguments(table.unpack(args)) then
      local result = parser:process(x + 15, y, table.unpack(args))
      for _,v in pairs(result) do
        table.insert(ret, v.baseTable)
      end
    end
  end
  return ret
end

return {
  identifier = "assign_item",
  processor = parseItemAssignment,
  arguments = {
    {
      name = "var",
      types = { "string" }
    },
    {
      name = "filter",
      types = { "item_filter" },
      required = false
    }
  }
}