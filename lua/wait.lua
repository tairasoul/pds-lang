local function parseWait(x, y, waitTime, type)
  local time = math.floor(waitTime)
  local str = tostring(time)
  if type ~= nil then
    str = str .. type
  end
  return {
    {
      name = "pneumaticcraft:wait",
      x = x,
      y = y,
      newX = x,
      newY = y + 11,
      width = 15,
      height = 11
    },
    {
      name = "pneumaticcraft:text",
      x = x + 15,
      y = y,
      newX = x,
      newY = y + 11,
      width = 15,
      height = 11,
      string = str
    }
  }
end

return {
  identifier = "wait",
  processor = parseWait,
  arguments = {
    {
      name = "waitTime",
      types = { "single" }
    },
    {
      name = "type",
      types = { "string" },
      required = false,
      validate = function(type)
        return type == "m" or type == "s"
      end
    }
  }
}