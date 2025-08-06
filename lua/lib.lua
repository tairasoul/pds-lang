function tableIncludes(tbl, itm)
  for _,v in pairs(tbl) do
    if v == itm then return true end
  end
  return false
end

function parseSides(sides)
  local count = 0
  local alreadyProcessed = {}
  for _,side in pairs(sides) do
    if tableIncludes(alreadyProcessed, side) then goto continue end
    alreadyProcessed = { table.unpack(alreadyProcessed), side }
    if side == "down" then
      count = count + 1
      goto continue
    end
    if side == "up" then
      count = count + 2
      goto continue
    end
    if side == "north" then
      count = count + 4
      goto continue
    end
    if side == "south" then
      count = count + 8
      goto continue
    end
    if side == "west" then
      count = count + 16
      goto continue
    end
    if side == "east" then
      count = count + 32
      goto continue
    end
    ::continue::
  end
  return count
end

function parseAxes(axes)
  local int = 0
  local alreadyProcessed = {}
  for _,v in pairs(axes) do
    if tableIncludes(alreadyProcessed, v) then goto continue end
    alreadyProcessed = { table.unpack(alreadyProcessed), v }
    if v == "x" then
      int = int + 1
      goto continue
    end
    if v == "y" then
      int = int + 2
      goto continue
    end
    if v == "z" then
      int = int + 4
      goto continue
    end
    ::continue::
  end
end

function parseCondition(cond)
  if cond == "==" then
    return "eq"
  end
  if cond == ">=" then
    return "ge"
  end
  if cond == "<=" then
    return "le"
  end
  if cond == "eq" or cond == "ge" or cond == "le" then return cond end
  return "eq"
end