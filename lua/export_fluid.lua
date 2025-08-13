local function parseParams(params)
  local sideInt = 0
  local use_count = false
  local count = 1
  local voidExcess = false
  if #params == 2 then
    local sides = params[1]
    sideInt = parseSides(sides)
    for _,v in pairs(params[2]) do
      if v == "use_count" then use_count = true goto continue end
      if v == "void_excess" then voidExcess = true goto continue end
      if string.match(v, "place=") then
        count = tonumber(_gsub(v, "place=", "")) goto continue
      end
      ::continue::
    end
  else
    local sides = params[1]
    sideInt = parseSides(sides)
  end
  return sideInt, use_count, count, voidExcess
end

local function parseImportFluid(x, y, areas, bareas, params, filters, bfilters)
  local s, uc, c, o, ve = parseParams(params)
  local widget = {
    name = "pneumaticcraft:liquid_export",
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
    },
    place_fluid_blocks = ve
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
  if bfilters ~= nil then
    local filterWidgets = {}
    index = 0
    for _,call in pairs(bfilters) do
      local parser = call.parser
      local args = call.objects
      if parser:validateArguments(table.unpack(args)) then
        index = index + 1
        local result = parser:process(x - 15 * index, y + 11, table.unpack(args))
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
  identifier = "export_fluid",
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
    },
    {
      name = "filters",
      types = { "fluid_filter[]" },
      required = false
    },
    {
      name = "blacklist_filters",
      types = { "fluid_filter[]" },
      required = false
    }
  }
}