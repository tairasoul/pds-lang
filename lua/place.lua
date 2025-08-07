local function processParams(params)
  local placeOrder = "closest"
  local useMaxActions = false
  local maxActions = 1
  local randomize = false
  if params == nil then return placeOrder, useMaxActions, maxActions, randomize end
  for _,param in pairs(params) do
    if string.match(param, "order=") then
      placeOrder = _gsub(param, "order=", "")
    end
    if string.match(param, "useMaxActions=") then
      useMaxActions = _gsub(param, "useMaxActions=", "") == "true"
    end
    if string.match(param, "maxActions=") then
      maxActions = tonumber(_gsub(param, "maxActions=", ""))
    end
    if string.match(param, "randomize=") then
      randomize = _gsub(param, "randomize=", "") == "true"
    end
  end
  return placeOrder, useMaxActions, maxActions, randomize
end

local function parseDig(x, y, areas, filters, params)
  local po, useMX, mx, rd = processParams(params)
  local areaWidgets = {}
  local index = 0
  for _,call in pairs(areas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(areaWidgets, formatted)
      end
    end
  end
  local filterWidgets = {}
  index = 0
  for _,call in pairs(filters) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(filterWidgets, formatted)
      end
    end
  end
  local widget = {
    name = "pneumaticcraft:place",
    x = x,
    y = y,
    newX = x,
    newY = y + 11,
    width = 15,
    height = 11,
    dig_place = {
      order = po,
      use_max_actions = useMX,
      max_actions = mx
    },
    randomize = rd
  }
  local res = {
    widget,
    table.unpack(areaWidgets),
    table.unpack(filterWidgets)
  }
  return res
end

return {
  identifier = "place",
  processor = parseDig,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "filters",
      types = { "item_filter[]" }
    },
    {
      name = "params",
      types = { "string[]" },
      required = false
    }
  }
}