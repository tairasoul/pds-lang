local function parseCoordinateCondition(x, y, cond, axes, coords1, coords2, truthy, falsy)
  local widget = {
    name = "pneumaticcraft:condition_coordinate",
    x = x,
    y = y,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 33,
    cond_op = parseCondition(cond),
    axis_options = {
      axes = parseAxes(axes)
    }
  }
  local index = 0
  local coordinateWidgets = {}
  for _,v in pairs(coords1) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,res in result do
        table.insert(coordinateWidgets, res.baseTable)
      end
    end
  end
  index = 0
  for _,v in pairs(coords2) do
    local parser = v.parser
    local args = v.objects
    if parser:validateArguments(table.unpack(args)) then
      local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
      for _,res in result do
        table.insert(coordinateWidgets, res.baseTable)
      end
    end
  end
  local truthyLabel = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y + 22,
    newX = x,
    newY = y + 33,
    width = 15,
    height = 11,
    string = truthy
  }
  local ret = {
    widget,
    table.unpack(coordinateWidgets),
    truthyLabel
  }
  if falsy ~= nil then
    local falsyLabel = {
      name = "pneumaticcraft:text",
      x = x - 15,
      y = y + 22,
      newX = x,
      newY = y + 33,
      width = 11,
      height = 15,
      string = falsy
    }
    table.insert(ret, falsyLabel)
  end
  return ret
end

return {
  identifier = "coordinate_condition",
  processor = parseCoordinateCondition,
  arguments = {
    {
      name = "cond",
      types = { "string" }
    },
    {
      name = "axes",
      types = { "string[]" }
    },
    {
      name = "coords1",
      types = { "coordinate[]" }
    },
    {
      name = "coords2",
      types = { "coordinate[]" }
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