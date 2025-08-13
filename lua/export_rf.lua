local function parseParams(params)
  local sideInt = 0
  local use_count = false
  local count = 1
  if #params == 2 then
    local sides = params[1]
    sideInt = parseSides(sides)
    for _,v in pairs(params[2]) do
      if v == "use_count" then use_count = true goto continue end
      if string.match(v, "count=") then
        count = tonumber(_gsub(v, "count=", "")) goto continue
      end
      ::continue::
    end
  else
    local sides = params[1]
    sideInt = parseSides(sides)
  end
  return sideInt, use_count, count
end

local function parseImportFluid(x, y, areas, bareas, params)
  local s, uc, c = parseParams(params)
  local widget = {
    name = "pneumaticcraft:rf_export",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    inv = {
      sides = s,
      use_count = uc,
      count = c
    }
  }
  local areaWidgets = {};
  local index = 0
  for _, call in pairs(areas) do
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
  for _, call in pairs(bareas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x - 15 * index, y, table.unpack(args))
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
  return ret
end

return {
  identifier = "export_rf",
  processor = parseImportFluid,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "blacklist_areas",
      types = { "area[]" }
    },
    {
      name = "params",
      types = { "object[]" }
    }
  }
}