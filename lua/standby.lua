local function parseStandby(x, y, allow_pickup)
  local widget = {
    name = "pneumaticcraft:standby",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
  if allow_pickup ~= nil then
    widget.allow_pickup = allow_pickup
  end
  return {
    widget
  }
end

return {
  identifier = "standby",
  processor = parseStandby,
  arguments = {
    {
      name = "allow_pickup",
      types = { "boolean" },
      required = false
    }
  }
}