local function parseSuicide(x, y)
  return {
    {
      name = "pneumaticcraft:suicide",
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
  identifier = "suicide",
  processor = parseSuicide
}