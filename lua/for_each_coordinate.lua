local function parseForEachCoordinate(x, y, areas, varName, label)
  local widget = {
    name = "pneumaticcraft:for_each_coordinate",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    var = varName
  }
  local text = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y + 11,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
    string = label
  }
  local areaWidgets = {}
  local index = 0
  for _,v in pairs(areas) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,res in pairs(result) do
        table.insert(areaWidgets, res.baseTable)
      end
    end
  end
  return {
    widget,
    text,
    table.unpack(areaWidgets)
  }
end

return {
  identifier = "for_each_coordinate",
  processor = parseForEachCoordinate,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "varName",
      types = { "string" }
    },
    {
      name = "label",
      types = { "string" }
    }
  }
}