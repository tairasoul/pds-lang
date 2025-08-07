local function parseParams(params)
  local useMaxActions = false
  local checkSight = false
  local maxActions = 1
  if params == nil then return useMaxActions, checkSight, maxActions end
  for _,v in pairs(params) do
    if v == "use_max_actions" then useMaxActions = true goto continue end
    if v == "check_sight" then checkSight = true goto continue end
    if string.match(v, "max_actions=") then
      maxActions = tonumber(_gsub(v, "max_actions=", ""))
      goto continue
    end
    ::continue::
  end
  return useMaxActions, checkSight, maxActions
end

local function parseAttackEntity(x, y, areas, params, entities)
  local umx, cs, mx = parseParams(params)
  local widget = {
    name = "pneumaticcraft:entity_attack",
    x = x,
    y = y,
    newX = x,
    newY = y + 22,
    width = 15,
    height = 22,
    use_max_actions = umx,
    check_sight = cs,
    max_actions = mx
  }
  local areaWidgets = {}
  local index = 0
  for _,call in pairs(areas) do
    local parser = call.parser
    local args = call.objects
    if parser:validateArguments(table.unpack(args)) then
      index = index + 1
      local result = parser:process(x + 15 * index, y, table.unpack(args))
      for _,resWidget in pairs(result) do
        table.insert(areaWidgets, resWidget.baseTable)
      end
    end
  end
  local ret = {
    widget,
    table.unpack(areaWidgets)
  }
  if entities ~= nil then
    index = 0
    local entityWidgets = {}
    for _,entity in pairs(entities) do
      index = index + 1
      local textWidget = {
        name = "pneumaticcraft:text",
        x = x + 15 * index,
        y = y + 11,
        newX = x,
        newY = y,
        width = 15,
        height = 11,
        string = entity
      }
      table.insert(entityWidgets, textWidget)
    end
    ret = { table.unpack(ret), table.unpack(entityWidgets) }
  end
  return ret
end

return {
  identifier = "attack_entity",
  processor = parseAttackEntity,
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
      name = "entities",
      types = { "string[]" },
      required = false
    }
  }
}