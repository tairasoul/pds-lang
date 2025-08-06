local function parsePickupItem(x, y, area, steal, filters)
  local widget = {
    name = "pneumaticcraft:pickup_item",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    can_steal = steal
  }
  local areaWidgets = {};
  local index = 0
  for _, call in pairs(area) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(areaWidgets, formatted)
      end
    end
  end
  local ret = {
    widget,
    table.unpack(areaWidgets)
  }
  if filters ~= nil then
    local filterWidgets = {}
    index = 0
    for _,call in pairs(filters) do
      local parser = call.parser
      local args = call.objects
      if parser:validateArguments(table.unpack(args)) then
        index = index + 1
        local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
        for _,resWidget in pairs(result) do
          local formatted = resWidget.baseTable
          table.insert(filterWidgets, formatted)
        end
      end
    end
    ret = { table.unpack(ret), table.unpack(filterWidgets) }
  end
  return ret
end

return {
  identifier = "pickup_item",
  processor = parsePickupItem,
  arguments = {
    {
      name = "area",
      types = { "area[]" }
    },
    {
      name = "steal",
      types = { "boolean" },
      required = false
    },
    {
      name = "filters",
      types = { "item_filter[]" },
      required = false
    }
  }
}