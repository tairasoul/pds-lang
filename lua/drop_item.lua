local function processParams(params)
  local use_count = false
  local count = 1
  local drop_straight = false
  local pick_delay = false
  if params == nil then return use_count, count, drop_straight, pick_delay end
  for _,param in pairs(params) do
    if param == "use_count" then use_count = true end
    if string.match(param, "count=") then
      count = tonumber(string.gsub(param, "count=", ""))
    end
    if param == "drop_straight" then drop_straight = true end
    if param == "pick_delay" then pick_delay = true end
  end
  return use_count, count, drop_straight, pick_delay
end

local function parseDropItem(x, y, area, params, filters)
  local uc, c, ds, pd = processParams(params)
  local widget = {
    name = "pneumaticcraft:drop_item",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    inv = {
      use_count = uc,
      count = c
    },
    drop_straight = ds,
    pick_delay = pd
  }
  local areaWidgets = {};
  local index = 0
  for _, call in pairs(area) do
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
  local ret = {
    widget,
    table.unpack(areaWidgets)
  }
  if filters ~= nil then
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
    ret = { table.unpack(ret), table.unpack(filterWidgets) }
  end
  return ret
end

return {
  identifier = "drop_item",
  processor = parseDropItem,
  arguments = {
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "params",
      types = { "string[]" },
      required = false
    },
    {
      name = "filters",
      types = { "item_filter[]" },
      required = false
    }
  }
}