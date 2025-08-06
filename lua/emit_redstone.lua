local function parseEmitRedstone(x, y, sides, level)
  local side = parseSides(sides)
  local widget = {
    name = "pneumaticcraft:emit_redstone",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    sides = side
  }
  local levelT = {
    name = "pneumaticcraft:text",
    x = x + 15,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    string = tostring(level)
  }
  return {
    widget, levelT
  }
end

return {
  identifier = "emit_redstone",
  processor = parseEmitRedstone,
  arguments = {
    {
      name = "sides",
      types = { "string[]" }
    },
    {
      name = "level",
      types = { "double" }
    }
  }
}