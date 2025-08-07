local function parseParams(params)
  local measure_var = nil
  local count = 1
  if params == nil then return measure_var, count end
  local alreadyProcessed = {}
  for _,v in pairs(params) do
    if tableIncludes(alreadyProcessed, v) then goto continue end
    table.insert(alreadyProcessed, v)
    if string.match(v, "measure_var=") then
      measure_var = _gsub(v, "measure_var=", "") goto continue
    end
    if string.match(v, "count=") then
      count = tonumber(_gsub(v, "count=", ""))
    end
    ::continue::
  end
  return measure_var, count
end

local function parseLightCondition(x, y, cond_op, filters, truthy, params, falsy)
  local mv, c  = parseParams(params)
  local widget = {
    name = "pneumaticcraft:drone_condition_item",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    drone_cond = {
      cond_op = parseCondition(cond_op),
      measure_var = mv
    },
    inv = {
      count = c
    }
  }
  local areaWidgets = {}
  local index = 0
  local filterWidgets = {}
  for _,v in pairs(filters) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _, res in pairs(result) do
        table.insert(filterWidgets, res.baseTable)
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
      height = 22,
      string = falsy
    }
    table.insert(ret, falsyText)
  end
  return ret
end

return {
  identifier = "drone_item_condition",
  processor = parseLightCondition,
  arguments = {
    {
      name = "cond_op",
      types = { "string" }
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