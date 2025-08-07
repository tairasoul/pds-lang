local function parseCoordOp(op)
  if op == "addsub" then return "plus_minus" end
  if op == "muldiv" then return "multiply_divide" end
  if op == "maxmin" then return "max_min" end
  if op == "plus_minus" or op == "multiply_divide" or op == "max_min" then return op end
  return "plus_minus"
end

local function parseCoordinateOperator(x, y, op, var, axes, left, right)
  local widget = {
    name = "pneumaticcraft:coordinate_operator",
    x = x,
    y = y,
    newX = x,
    newY = 11,
    width = 15,
    height = 11,
    var = var,
    axis_options = {
      axes = parseAxes(axes)
    },
    coord_op = parseCoordOp(op)
  }
  local ret = { widget }
  local index = 0
  if #left > 0 then
    for _,v in pairs(left) do
      local parser = v.parser
      local args = v.objects
      if parser:validateArguments(table.unpack(args)) then
        index = index + 1
        local result = parser:process(x - 15 * index, y, table.unpack(args))
        for _,res in pairs(result) do
          table.insert(ret, res.baseTable)
        end
      end
    end
    index = 0
  end
  if #right > 0 then
    for _,v in pairs(right) do
      local parser = v.parser
      local args = v.objects
      if parser:validateArguments(table.unpack(args)) then
        index = index + 1
        local result = parser:process(x + 15 * index, y, table.unpack(args))
        for _,res in pairs(result) do
          table.insert(ret, res.baseTable)
        end
      end
    end
  end
  return ret
end

return {
  identifier = "coordinate_operator",
  processor = parseCoordinateOperator,
  arguments = {
    {
      name = "op",
      types = { "string" }
    },
    {
      name = "var",
      types = { "string" }
    },
    {
      name = "axes",
      types = { "string[]" }
    },
    {
      name = "left",
      types = { "coordinate[]" }
    },
    {
      name = "right",
      types = { "coordinate[]" }
    }
  }
}