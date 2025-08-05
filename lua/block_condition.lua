local function formatWidget(result)
  return result.baseTable
end

local function parseParams(params)
  local andFunc = false
  local checkAir = false
  local checkLiquid = false
  local measureVar = nil
  for _,v in pairs(params) do
    if v == "and" then andFunc = true end
    if v == "air" then checkAir = true end
    if v == "liquid" then checkLiquid = true end
    if string.match(v, "measure&") then
      local varName = string.gsub(v, "measure&", "")
      measureVar = varName
    end
  end
  return andFunc, checkAir, checkLiquid, measureVar
end

local function parseBlockCondition(x, y, params, coords, filters, truthy, falsy)
  local af, ca, cl, mv = parseParams(params)
  local widget = {
    name = "pneumaticcraft:condition_block",
    x = x,
    y = y,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 33,
    cond = {
      and_func = af,
      measure_var = mv
    },
    inv = { "__EMPTY_TABLE" },
    check_air = ca,
    check_liquid = cl
  }
  local areaWidgets = {}
  local index = 0
  for _,call in pairs(coords) do
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
  index = 0
  local filterWidgets = {}
  for _,call in pairs(filters) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = formatWidget(resWidget)
        table.insert(filterWidgets, formatted)
      end
    end
  end
  local truthyWidget = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y + 22,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
    string = truthy
  }
  local ret = {
    widget,
    table.unpack(areaWidgets),
    table.unpack(filterWidgets),
    truthyWidget
  }
  if falsy ~= nil then
    table.insert(ret, {
      name = "pneumaticcraft:text",
      x = x - 15,
      y = y + 22,
      newX = x,
      newY = y,
      width = 15,
      height = 11,
      string = falsy
    })
  end
  return ret
end

return {
  identifier = "block_condition",
  processor = parseBlockCondition,
  arguments = {
    {
      name = "param",
      types = { "object[]" }
    },
    {
      name = "coords",
      types = { "area[]" }
    },
    {
      name = "filters",
      types = { "item_filter[]" }
    },
    {
      name = "truthy",
      types = { "string" }
    },
    {
      name = "falsy",
      types = { "string" },
      required = false
    }
  }
}