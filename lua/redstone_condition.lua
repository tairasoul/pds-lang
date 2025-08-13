local function parseParams(params)
  local and_func = false
  local measure_var = nil
  local count = 1
  if params == nil then return and_func, measure_var, count end
  local alreadyProcessed = {}
  for _,v in pairs(params) do
    if tableIncludes(alreadyProcessed, v) then goto continue end
    table.insert(alreadyProcessed, v)
    if v == "and" then and_func = true goto continue end
    if string.match(v, "measure_var=") then
      measure_var = _gsub(v, "measure_var=", "") goto continue
    end
    if string.match(v, "count=") then
      count = tonumber(_gsub(v, "count=", ""))
    end
    ::continue::
  end
  return and_func, measure_var, count
end

local function parseRedstoneCondition(x, y, cond_op, sides, areas, bareas, truthy, params, falsy)
  local af, mv, c  = parseParams(params)
  local widget = {
    name = "pneumaticcraft:condition_redstone",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    cond = {
      cond_op = parseCondition(cond_op),
      and_func = af,
      measure_var = mv
    },
    inv = {
      sides = parseSides(sides),
      count = c
    }
  }
  local areaWidgets = {}
  local index = 0
  for _,v in pairs(areas) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _, res in pairs(result) do
        table.insert(areaWidgets, res.baseTable)
      end
    end
  end
  index = 0
  for _,v in pairs(bareas) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x - 15 * index, y, table.unpack(args))
      for _, res in pairs(result) do
        table.insert(areaWidgets, res.baseTable)
      end
    end
  end
  local truthyText = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y + 11,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 11,
    string = truthy
  }
  local ret = {
    widget,
    table.unpack(areaWidgets),
    truthyText
  }
  if falsy ~= nil then
    local falsyText = {
      name = "pneumaticcraft:text",
      x = x - 15,
      y = y + 11,
      newX = x,
      newY = y + 22,
      width = 15,
      height = 11,
      string = falsy
    }
    table.insert(ret, falsyText)
  end
  return ret
end

return {
  identifier = "redstone_condition",
  processor = parseRedstoneCondition,
  arguments = {
    {
      name = "cond_op",
      types = { "string" }
    },
    {
      name = "sides",
      types = { "string[]" }
    },
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "blacklist_areas",
      types = { "area[]" }
    },
    {
      name = "truthy",
      types = { "string" }
    },
    {
      name = "params",
      types = { "string[]" },
      required = false
    },
    {
      name = "falsy",
      types = { "string" },
      required = false
    }
  }
}