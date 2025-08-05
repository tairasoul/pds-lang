local function parseCoordinate(x, y, coord)
  local widget = {
    name = "pneumaticcraft:coordinate",
    x = x,
    y = y,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
  }
  if type(coord) == "string" then
    widget.var = coord
    widget.usingVar = true
  else
    widget.coord = coord
  end
  return widget
end

return {
  identifier = "coordinate",
  processor = parseCoordinate,
  arguments = {
    {
      name = "coord",
      types = { "string", "table" }
    }
  },
  validOutsideArguments = false
}