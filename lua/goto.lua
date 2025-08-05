local function formatWidget(result)
  return result.baseTable
end

local function parseGoto(x, y, areas, done_when_depart)
  local widget = {
    name = "pneumaticcraft:goto",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
  if done_when_depart ~= nil then
    widget.done_when_depart = done_when_depart
  end
  local areaWidgets = {}
  local index = 0
  for i,call in ipairs(areas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = formatWidget(resWidget)
        table.insert(areaWidgets, formatted)
      end
    end
  end
  return {
    widget,
    table.unpack(areaWidgets)
  }
end

return {
  identifier = "goto",
  processor = parseGoto,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "done_when_depart",
      types = { "boolean" },
      required = false
    }
  }
}