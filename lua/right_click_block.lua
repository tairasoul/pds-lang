local function parseBlockRightClick(x, y, side, areas, filters, click_type, sneaking)
  local widget = {
    name = "pneumaticcraft:block_right_click",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    side = side,
    click_type = click_type,
    sneaking = sneaking
  }
  local filterWidgets = {}
  local index = 0
  for _,call in ipairs(filters) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(filterWidgets, formatted)
      end
    end
  end
  local areaWidgets = {}
  index = 0
  for _,call in ipairs(areas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y + 11, table.unpack(args))
      for _,resWidget in pairs(result) do
        local formatted = resWidget.baseTable
        table.insert(areaWidgets, formatted)
      end
    end
  end
  local res = {
    widget,
    table.unpack(areaWidgets)
  }
  if filters ~= nil then
    res = { table.unpack(res), table.unpack(filterWidgets) }
  end
  return res
end

return {
  identifier = "right_click_block",
  processor = parseBlockRightClick,
  arguments = {
    {
      name = "side",
      types = { "string" },
      validate = function(side)
        return side == "up" or side == "north" or side == "east" or side == "west" or side == "south" or side == "down"
      end
    },
    {
      name = "areas",
      types = { "area[]" }
    },
    {
      name = "filters",
      types = { "item_filter[]" },
      required = false
    },
    {
      name = "click_type",
      types = { "string" },
      validate = function(type)
        return type == "click_block" or type == "click_item"
      end,
      required = false
    },
    {
      name = "sneaking",
      types = { "bool" },
      required = false
    }
  }
}