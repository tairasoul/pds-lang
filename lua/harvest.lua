local function processParams(params)
  local placeOrder = "closest"
  local useMaxActions = false
  local maxActions = 1
  local requireTool = false
  if params == nil then return placeOrder, useMaxActions, maxActions, requireTool end
  for _,param in pairs(params) do
    if string.match(param, "order=") then
      placeOrder = string.gsub(param, "order=", "")
    end
    if string.match(param, "useMaxActions=") then
      useMaxActions = string.gsub(param, "useMaxActions=", "") == "true"
    end
    if string.match(param, "maxActions=") then
      maxActions = tonumber(string.gsub(param, "maxActions=", ""))
    end
    if param == "requireTool" then
      requireTool = true
    end
  end
  return placeOrder, useMaxActions, maxActions, requireTool
end

local function parseDig(x, y, areas, params, filters)
  local po, useMX, mx, rt = processParams(params)
  local areaWidgets = {}
  local index = 0
  for _,call in ipairs(areas) do
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
  if filters ~= nil then
    index = 0
    for _,call in ipairs(filters) do
      local parser = call.parser
      local args = call.objects
      if parser:validateArguments(args) then
        index = index + 1
        local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
        for _,resWidget in pairs(result) do
          local formatted = resWidget.baseTable
          table.insert(filterWidgets, formatted)
        end
      end
    end
  end
  local widget = {
    name = "pneumaticcraft:harvest",
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
    require_hoe = rt
  }
  local res = {
    widget,
    table.unpack(areaWidgets),
  }
  if filters ~= nil then
    res = { table.unpack(res), table.unpack(filterWidgets) };
  end
  return res
end

return {
  identifier = "harvest",
  processor = parseDig,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "params",
      types = { "object[]" },
      required = false
    },
    {
      name = "filters",
      types = { "item_filter[]" },
      required = false
    }
  }
}