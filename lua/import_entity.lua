local function parseRightClickEntity(x, y, areas, entities)
  local widget = {
    name = "pneumaticcraft:entity_import",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22
  }
  local areaWidgets = {}
  local index = 0
  for _,call in pairs(areas) do
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
  index = 0
  local entityWidgets = {}
  for _,entity in pairs(entities) do
    index = index + 1
    local textWidget = {
      name = "pneumaticcraft:text",
      x = x + 15 * index,
      y = y + 11,
      newX = x,
      newY = y,
      width = 15,
      height = 11,
      string = entity
    }
    table.insert(entityWidgets, textWidget)
  end
  return {
    widget,
    table.unpack(areaWidgets),
    table.unpack(entityWidgets)
  }
end

return {
  identifier = "import_entity",
  processor = parseRightClickEntity,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "entities",
      types = { "string[]" }
    }
  }
}