local function parseRenameDrone(x, y, name)
  local widget = {
    name = "pneumaticcraft:rename",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
  return {
    widget,
    {
      name = "pneumaticcraft:text",
      x = x + 15,
      y = y,
      newX = x,
      newY = y + 11,
      width = 15,
      height = 11,
      string = name
    }
  }
end

return {
  identifier = "rename",
  processor = parseRenameDrone,
  arguments = {
    {
      name = "name",
      types = { "string" }
    }
  }
}