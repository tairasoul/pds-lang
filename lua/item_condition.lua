local function parseLightCondition(x, y, filter, filters, truthy, falsy)
  local widget = {
    name = "pneumaticcraft:condition_item",
    x = x,
    y = y,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 33
  }
  local index = 0
  local filterWidgets = {}
  for _,v in pairs(filters) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
      for _, res in pairs(result) do
        table.insert(filterWidgets, res.baseTable)
      end
    end
  end
  local truthyText = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y + 22,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 11,
    string = truthy
  }
  local ret = {
    widget,
    table.unpack(filterWidgets),
    truthyText
  }
  local parser = filter.parser
  local args = filter.objects
  if parser:validateArguments(table.unpack(args)) then
    local result = parser:process(x + 15, y, table.unpack(args))
    for _, res in pairs(result) do
      table.insert(ret, res)
    end
  end
  if falsy ~= nil then
    local falsyText = {
      name = "pneumaticcraft:text",
      x = x - 15,
      y = y + 11,
      newX = x,
      newY = y + 33,
      width = 15,
      height = 22,
      string = falsy
    }
    table.insert(ret, falsyText)
  end
  return ret
end

return {
  identifier = "item_condition",
  processor = parseLightCondition,
  arguments = {
    {
      name = "filter",
      types = { "item_filter" }
    },
    {
      name = "filters",
      types = { "item_filter[]" }
    },
    {
      name = "truthy",
      types = { "string" }
    },
    {
      name = "params",
      types = { "string[]" },
      required = false
    },
    {
      name = "falsy",
      types = { "string" },
      required = false
    }
  }
}