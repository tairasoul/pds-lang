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

local function parseLightCondition(x, y, cond_op, truthy, params, falsy)
  local mv, c  = parseParams(params)
  local widget = {
    name = "pneumaticcraft:drone_condition_rf",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    cond = {
      cond_op = parseCondition(cond_op),
      measure_var = mv
    },
    inv = {
      count = c
    }
  }
  local truthyText = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    string = truthy
  }
  local ret = {
    widget,
    truthyText
  }
  if falsy ~= nil then
    local falsyText = {
      name = "pneumaticcraft:text",
      x = x - 15,
      y = y,
      newX = x,
      newY = y + 11,
      width = 15,
      height = 11,
      string = falsy
    }
    table.insert(ret, falsyText)
  end
  return ret
end

return {
  identifier = "drone_rf_condition",
  processor = parseLightCondition,
  arguments = {
    {
      name = "cond_op",
      types = { "string" }
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