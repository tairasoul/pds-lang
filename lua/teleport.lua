local function formatWidget(result)
  local widget = {}
  for key,val in pairs(result.baseTable) do
    widget[key] = val
  end
  return widget
end

local function parseGoto(x, y, areas, bareas, done_when_depart)
  local widget = {
    name = "pneumaticcraft:teleport",
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
    if parser:validateArguments(args) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = formatWidget(resWidget)
        table.insert(areaWidgets, formatted)
      end
    end
  end
  index = 0
  if bareas ~= nil then
    for i,call in ipairs(bareas) do
      local parser = call.parser
      local args = call.objects
      if parser:validateArguments(args) then
        index = index + 1
        local result = parser:process(x - 15 * index, y, table.unpack(args))
        for _,resWidget in pairs(result) do
          local formatted = formatWidget(resWidget)
          table.insert(areaWidgets, formatted)
        end
      end
    end
  end
  return {
    widget,
    table.unpack(areaWidgets)
  }
end

return {
  identifier = "teleport",
  processor = parseGoto,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "blacklist_areas",
      types = { "area[]" },
      required = false
    },
    {
      name = "done_when_depart",
      types = { "boolean" },
      required = false
    }
  }
}