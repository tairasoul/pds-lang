if not gExists("startAdded") then
  startAdded = false
end

local function parseStart(x, y)
  if startAdded then
    error("Start has already been added to this file!")
    return nil
  end
  startAdded = true
  return {
    {
      name = "pneumaticcraft:start",
      x = x,
      y = y,
      newX = x,
      newY = y + 11,
      width = 15,
      height = 11
    }
  }
end

return {
  identifier = "start",
  processor = parseStart
}