local function formatWidget(result)
  return result.baseTable
end

local function parseGoto(x, y, areas, bareas)
  local widget = {
    name = "pneumaticcraft:logistics",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
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
  identifier = "logistics",
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
    }
  }
}