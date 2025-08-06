local function processItemFilter(x, y, fluid, count)
  local widget = {
    name = "pneumaticcraft:fluid_filter",
    x = x,
    y = y,
    newX = x,
    newY = y,
    width = 15,
    height = 11,
    fluid = {
      id = fluid,
      count = count or 1
    }
  }
  return {
    widget
  }
end

return {
  identifier = "fluid_filter",
  processor = processItemFilter,
  arguments = {
    {
      name = "fluid",
      types = { "string" }
    },
    {
      name = "count",
      types = { "double" },
      required = false
    }
  },
  validOutsideArguments = false
}