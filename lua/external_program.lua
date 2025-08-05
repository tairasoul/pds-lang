local function formatWidget(result)
  local widget = {}
  for key,val in pairs(result.baseTable) do
    widget[key] = val
  end
  return widget
end

local function parseGoto(x, y, areas, share_variables)
  local widget = {
    name = "pneumaticcraft:external_program",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11
  }
  if share_variables ~= nil then
    widget.share_variables = share_variables
  end
  local areaWidgets = {}
  local index = 0
  for i,call in ipairs(areas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(args) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = formatWidget(resWidget)
        table.insert(areaWidgets, formatted)
      end
    end
  end
  return {
    widget,
    table.unpack(areaWidgets)
  }
end

return {
  identifier = "external_program",
  processor = parseGoto,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "share_variables",
      types = { "boolean" },
      required = false
    }
  }
}