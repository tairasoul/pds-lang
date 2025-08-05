local function parseArea(x, y, arr1, arr2, mode)
  local area_type
  if #arr1 ~= 3 and not type(arr1) == "string" then
    error("Coordinate array 1 should be 3 numbers (x, y, z) or global variable!");
  end
  if #arr2 ~= 3 and not type(arr2) == "string" then
    error("Coordinate array 2 should be 3 numbers (x, y, z) or global variable!");
  end
  local match = string.match(mode, ":")
  if match ~= nil then
    area_type = mode
  else
    area_type = "pneumaticcraft:" .. mode
  end
  local widget = {
    name = "pneumaticcraft:area",
    x = x,
    y = y,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
    area_type = {
      type = area_type
    }
  }
  if type(arr1) == "string" then widget.var1 = arr1 else widget.pos1 = arr1 end
  if type(arr2) == "string" then widget.var1 = arr2 else widget.pos2 = arr2 end
  return {
    widget
  }
end

return {
  identifier = "area",
  processor = parseArea,
  arguments = {
    {
      name = "pos1",
      types = { "string", "table" }
    },
    {
      name = "pos2",
      types = { "string", "table" }
    },
    {
      name = "mode",
      types = { "string" }
    }
  },
  validOutsideArguments = false
}