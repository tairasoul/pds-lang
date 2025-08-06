local function tableIncludes(tbl, itm)
  for _,v in pairs(tbl) do
    if v == itm then return true end
  end
  return false
end

local function parseSides(sides)
  local count = 0
  local alreadyProcessed = {}
  for _,side in pairs(sides) do
    if tableIncludes(alreadyProcessed, side) then goto continue end
    alreadyProcessed = { table.unpack(alreadyProcessed), side }
    if side == "down" then
      count = count + 1
      goto continue
    end
    if side == "up" then
      count = count + 2
      goto continue
    end
    if side == "north" then
      count = count + 4
      goto continue
    end
    if side == "south" then
      count = count + 8
      goto continue
    end
    if side == "west" then
      count = count + 16
      goto continue
    end
    if side == "east" then
      count = count + 32
      goto continue
    end
    ::continue::
  end
  return count
end

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
        count = tonumber(string.gsub(v, "count=", "")) goto continue
      end
      ::continue::
    end
  else
    local sides = params[1]
    sideInt = parseSides(sides)
  end
  return sideInt, use_count, count
end

local function parseImportFluid(x, y, area, params)
  local s, uc, c = parseParams(params)
  local widget = {
    name = "pneumaticcraft:rf_import",
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
  return ret
end

return {
  identifier = "import_rf",
  processor = parseImportFluid,
  arguments = {
    {
      name = "area",
      types = { "area[]" }
    },
    {
      name = "params",
      types = { "object[]" }
    }
  }
}