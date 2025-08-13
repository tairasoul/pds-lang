local function parseVoidFluid(x, y, filters, bfilters)
  local widget = {
    name = "pneumaticcraft:void_liquid",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
  local filterWidgets = {}
  local index = 0
  for _,call in ipairs(filters) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(filterWidgets, formatted)
      end
    end
  end
  index = 0
  if bfilters ~= nil then
    for _,call in ipairs(bfilters) do
      local parser = call.parser
      local args = call.objects
      if parser:validateArguments(table.unpack(args)) then
        index = index + 1
        local result = parser:process(x - 15 * index, y, table.unpack(args))
        for _,resWidget in pairs(result) do
          local formatted = resWidget.baseTable
          table.insert(filterWidgets, formatted)
        end
      end
    end
  end
  return {
    widget,
    table.unpack(filterWidgets)
  }
end

return {
  identifier = "void_fluid",
  processor = parseVoidFluid,
  arguments = {
    {
      name = "filters",
      types = { "fluid_filter[]" }
    },
    {
      name = "blacklist_filters",
      types = { "fluid_filter[]" },
      required = false
    }
  }
}