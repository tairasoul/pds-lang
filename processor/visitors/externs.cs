using tairasoul.pdsl.visitors.errors;
using tairasoul.pdsl.visitors.returnTypes;
using statement = tairasoul.pdsl.ast.statements.Statement;
using expression = tairasoul.pdsl.ast.expressions.Expression;
using tairasoul.pdsl.visitors.util;
using tairasoul.pdsl.processor;

namespace tairasoul.pdsl.visitors.externs;

static class StartExternVisitor 
{
  public static Widget process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    return new Widget("pneumaticcraft:start", new() { x = lastPos.x, y = lastPos.y });
  }
}

static class NoArgumentExternVisitor 
{
  public static Widget process(string name, pieces.Coordinate lastPos, pieces.Coordinate lastSize) 
  {
    return new Widget($"pneumaticcraft:{name}", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
  }
}

static class CoordinateExternVisitor 
{
  public static Widget process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    Widget coord = new("pneumaticcraft:coordinate", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    if (@extern.parameters[0] is statement.Array coords) 
    {
      object[] coordParams = VisitorUtil.GetLiteralParams(coords.expressions, "coordinate", [(typeof(float), null), (typeof(float), null), (typeof(float), null)]);
      float xCoord = (float)coordParams[0];
      float yCoord = (float)coordParams[1];
      float zCoord = (float)coordParams[2];
      int x = (int)xCoord;
      int y = (int)yCoord;
      int z = (int)zCoord;
      coord.ExtraProperties["coord"] = new int[] { x, y, z };
    }
    else if (@extern.parameters[0] is statement.Expression varExpr) 
    {
      expression val = varExpr.expression;
      if (val is not expression.Literal) 
      {
        throw new VisitorError($"Expected literal for global variable expression, got {val.GetType().Name}", varExpr);
      }
      expression.Literal literal = (expression.Literal) val;
      if (literal.value is not string globalVar) 
      {
        throw new VisitorError($"Expected string for global variable expression, got {literal.value.GetType().Name}", varExpr);
      }
      coord.ExtraProperties["var"] = globalVar;
      coord.ExtraProperties["using_var"] = true;
    }
    else 
    {
      throw new VisitorError($"Expected string literal or [x, y, z], got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    return coord;
  }
}

static class AreaExternVisitor 
{
  public static Widget process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement[] arguments) 
  {
    Widget widget = new("pneumaticcraft:area", new() { x = lastPos.x + lastSize.x, y = lastPos.y });
    statement firstarr = arguments[0];
    if (firstarr is not statement.Array && firstarr is not statement.Expression) 
    {
      throw new VisitorError($"Expected array or global variable name as first argument, got {firstarr.GetType().Name}", firstarr);
    }
    statement secondarr = arguments[1];
    if (secondarr is not statement.Array && firstarr is not statement.Expression) 
    {
      throw new VisitorError($"Expected array or global variable name as second argument, got {secondarr.GetType().Name}", secondarr);
    }
    statement areaType = arguments[2];
    string type = "box";
    if (areaType is not null && areaType is not statement.Expression) {
      throw new VisitorError($"Expected string as third argument, got {areaType.GetType().Name}", areaType);
    }
    if (areaType is not null) 
    {
      statement.Expression expr = (statement.Expression)areaType;
      expression val = expr.expression;
      if (val is not expression.Literal) 
      {
        throw new VisitorError($"Expected literal as third argument, got {val.GetType().Name}", expr);
      }
      expression.Literal literal = (expression.Literal)val;
      if (literal.value is not string) 
      {
        throw new VisitorError($"Expected string as third argument, got {literal.value.GetType().Name}", expr);
      }
      type = (string)literal.value;
    }
    widget.ExtraProperties["area_type"] = new { type = type.Any((c) => c == ':') ? type : $"pneumaticcraft:{type}" };
    if (firstarr is statement.Expression exp) 
    {
      expression val = exp.expression;
      if (val is not expression.Literal) 
      {
        throw new VisitorError($"Expected literal for global variable expression, got {val.GetType().Name}", exp);
      }
      expression.Literal literal = (expression.Literal) val;
      if (literal.value is not string) 
      {
        throw new VisitorError($"Expected string for global variable expression, got {val.GetType().Name}", exp);
      }
      widget.ExtraProperties["var1"] = literal.value;
    }
    else if (firstarr is statement.Array arr) 
    {
      statement x = arr.expressions[0];
      statement y = arr.expressions[1];
      statement z = arr.expressions[2];
      if (x is not statement.Expression exprX)
        throw new VisitorError($"Expected expression for point X, got {x.GetType().Name}", x);
      if (y is not statement.Expression exprY)
        throw new VisitorError($"Expected expression for point Y, got {y.GetType().Name}", y);
      if (z is not statement.Expression exprZ)
        throw new VisitorError($"Expected expression for point Z, got {z.GetType().Name}", z);
      expression xE = exprX.expression;
      expression yE = exprY.expression;
      expression zE = exprZ.expression;
      if (xE is not expression.Literal xLE)
        throw new VisitorError($"Expected literal for point X, got {xE.GetType().Name}", x);
      if (yE is not expression.Literal yLE)
        throw new VisitorError($"Expected literal for point Y, got {yE.GetType().Name}", y);
      if (zE is not expression.Literal zLE)
        throw new VisitorError($"Expected literal for point Z, got {zE.GetType().Name}", z);
      object xV = xLE.value;
      object yV = yLE.value;
      object zV = zLE.value;
      if (xV is not float xS)
        throw new VisitorError($"Expected number for point X, got {xV.GetType().Name}", x);
      if (yV is not float yS)
        throw new VisitorError($"Expected number for point Y, got {yV.GetType().Name}", y);
      if (zV is not float zS)
        throw new VisitorError($"Expected number for point Z, got {zV.GetType().Name}", z);
      widget.ExtraProperties["pos1"] = new int[] { (int)xS, (int)yS, (int)zS };
    }
    if (secondarr is statement.Expression exp2) 
    {
      expression val = exp2.expression;
      if (val is not expression.Literal) 
      {
        throw new VisitorError($"Expected literal for global variable expression, got {val.GetType().Name}", exp2);
      }
      expression.Literal literal = (expression.Literal) val;
      if (literal.value is not string) 
      {
        throw new VisitorError($"Expected string for global variable expression, got {val.GetType().Name}", exp2);
      }
      widget.ExtraProperties["var2"] = literal.value;
    }
    else if (secondarr is statement.Array arr) 
    {
      statement x = arr.expressions[0];
      statement y = arr.expressions[1];
      statement z = arr.expressions[2];
      if (x is not statement.Expression exprX)
        throw new VisitorError($"Expected expression for point X, got {x.GetType().Name}", x);
      if (y is not statement.Expression exprY)
        throw new VisitorError($"Expected expression for point Y, got {y.GetType().Name}", y);
      if (z is not statement.Expression exprZ)
        throw new VisitorError($"Expected expression for point Z, got {z.GetType().Name}", z);
      expression xE = exprX.expression;
      expression yE = exprY.expression;
      expression zE = exprZ.expression;
      if (xE is not expression.Literal xLE)
        throw new VisitorError($"Expected literal for point X, got {xE.GetType().Name}", x);
      if (yE is not expression.Literal yLE)
        throw new VisitorError($"Expected literal for point Y, got {yE.GetType().Name}", y);
      if (zE is not expression.Literal zLE)
        throw new VisitorError($"Expected literal for point Z, got {zE.GetType().Name}", z);
      object xV = xLE.value;
      object yV = yLE.value;
      object zV = zLE.value;
      if (xV is not float xS)
        throw new VisitorError($"Expected number for point X, got {xV.GetType().Name}", x);
      if (yV is not float yS)
        throw new VisitorError($"Expected number for point Y, got {yV.GetType().Name}", y);
      if (zV is not float zS)
        throw new VisitorError($"Expected number for point Z, got {zV.GetType().Name}", z);
      widget.ExtraProperties["pos2"] = new int[] { (int)xS, (int)yS, (int)zS };
    }
    return widget;
  }
}

static class ItemFilterExtern 
{
  public static Widget process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern filter) 
  {
    statement first = filter.parameters[0];
    if (first is not statement.Expression firstExpr) 
    {
      throw new VisitorError($"Expected expression for first filter argument, got {first.GetType().Name}", first);
    }
    statement? second = @filter.parameters.Length > 1 ? @filter.parameters[1] : null;
    if (second is not statement.Expression && second is not null) 
    {
      throw new VisitorError($"Expected expression for second filter argument, got {first.GetType().Name}", second);
    }
    statement.Expression? secondExpr = (statement.Expression?)second;
    if (firstExpr.expression is not expression.Literal firstLit) 
    {
      throw new VisitorError($"Expected literal for first filter argument, got {first.GetType().Name}", firstExpr);
    }
    if (secondExpr is not null && secondExpr.expression is not expression.Literal) 
    {
      throw new VisitorError($"Expected literal for second filter argument, got {first.GetType().Name}", secondExpr);
    }
    expression.Literal? secondLit = (expression.Literal?)(secondExpr?.expression);
    if (firstLit.value is not string itemName) 
    {
      throw new VisitorError($"Expected string for filter item, got {first.GetType().Name}", firstExpr);
    }
    if (secondLit is not null && secondLit.value is not string) {
      throw new VisitorError($"Expected string for filter options, got {first.GetType().Name}", secondExpr!);
    }
    Widget widget = new($"pneumaticcraft:item_filter", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? filterOptions = (string?)(secondLit?.value);
    bool componentsEnabled = false;
    bool modEnabled = false;
    bool blockEnabled = false;
    bool durabilityEnabled = false;
    if (filterOptions != null) 
    {
      string[] options = filterOptions.Split(";");
      foreach (string option in options) 
      {
        if (option == "components")
          componentsEnabled = true;
        else if (option == "mod")
          modEnabled = true;
        else if (option == "block")
          blockEnabled = true;
        else if (option == "durability")
          durabilityEnabled = true;
      }
    }
    widget.ExtraProperties["chk_durability"] = durabilityEnabled;
    widget.ExtraProperties["chk_block"] = blockEnabled;
    widget.ExtraProperties["chk_mod"] = modEnabled;
    widget.ExtraProperties["chk_components"] = componentsEnabled;
    if (itemName.StartsWith("var ")) 
    {
      widget.ExtraProperties["var"] = itemName.Replace("var ", "");
    }
    else 
    {
      string[] split = itemName.Split("&");
      int count = int.Parse(split.Length > 1 ? split[1] : "1");
      string item = split[0];
      widget.ExtraProperties["chk_item"] = new
      {
        id = item,
        count
      };
    }
    return widget;
  }
}

static class PrimaryMovementExterns 
{
  public static Widget[] process(string name, pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement firstParam = @extern.parameters[0];
    if (firstParam is not statement.Array areasArr) 
    {
      throw new VisitorError($"Expected array of areas, got {firstParam.GetType().Name}", firstParam);
    }
    statement.Expression? doneWhenDepart = 
      @extern.parameters.Length > 1
      ? ((@extern.parameters[1] is statement.Expression) ? @extern.parameters[1] as statement.Expression : throw new VisitorError($"Expected expression for {name} parameter, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1])) : null;
    bool doneWhenDepartVal = doneWhenDepart is not null
      ? doneWhenDepart.expression is expression.Literal { value: bool intVal }
          ? intVal
          : throw new VisitorError($"Expected integer literal for {name} parameter, got {doneWhenDepart.expression.GetType().Name}", doneWhenDepart)
      : false;
    Widget widget = new($"pneumaticcraft:{name}", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    widget.ExtraProperties["done_when_depart"] = doneWhenDepartVal;
    if (name == "computer_control")
      widget.ExtraProperties["inv"] = new {};
    Widget[] areas = [];
    int posMult = 0;
    foreach (statement.Extern statements in areasArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y;
      area.pos.x = lastPos.x + pieces.GoToLocation.Area.x * posMult;
      areas = [..areas, area];
    }
    return [widget, ..areas];
  }
}

static class FluidFilterExtern 
{
    public static Widget process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern filter) 
  {
    Widget widget = new($"pneumaticcraft:liquid_filter", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    object[] parameters = VisitorUtil.GetLiteralParams(filter.parameters, "liquid filter", [(typeof(string), null), (typeof(float), 1000f)]);
    string liquidId = (string)parameters[0];
    float liquidCount = (float)parameters[1];
    int liquidAmt = (int)liquidCount;
    widget.ExtraProperties["fluid"] = new
    {
      id = liquidId,
      amount = liquidAmt
    };
    return widget;
  }
}

static class VoidFluidExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    Widget widget = new($"pneumaticcraft:void_liquid", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    statement firstParam = @extern.parameters[0];
    if (firstParam is not statement.Array areasArr) 
    {
      throw new VisitorError($"Expected array of liquid filters, got {firstParam.GetType().Name}", firstParam);
    }
    Widget[] filters = [];
    int posMult = 0;
    foreach (statement.Extern statements in areasArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filter = FluidFilterExtern.process(lastPos, lastSize, statements);
      filter.pos.y = lastPos.y + lastSize.y;
      filter.pos.x = lastPos.x + pieces.VoidFluid.FluidFilter.x * posMult;
      filters = [..filters, filter];
    }
    return [widget, ..filters];
  }
}

static class VoidItemExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    Widget widget = new($"pneumaticcraft:void_item", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    statement firstParam = @extern.parameters[0];
    if (firstParam is not statement.Array areasArr) 
    {
      throw new VisitorError($"Expected array of filters, got {firstParam.GetType().Name}", firstParam);
    }
    Widget[] filters = [];
    int posMult = 0;
    foreach (statement.Extern statements in areasArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filter = ItemFilterExtern.process(lastPos, lastSize, statements);
      filter.pos.y = lastPos.y + lastSize.y;
      filter.pos.x = lastPos.x + pieces.VoidItem.Filter.x * posMult;
      filters = [..filters, filter];
    }
    return [widget, ..filters];
  }
}

static class BlockRightClickExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first block right click argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second block right click argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third block right click argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:block_right_click", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    object[] brcParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "block_right_click", [(typeof(string), "lowToHigh"), (typeof(string), "up"), (typeof(string), "click_block"), (typeof(bool), false)]);
    Widget[] allWidgets = [widget];
    widget.ExtraProperties["order"] = (string)brcParams[0];
    widget.ExtraProperties["side"] = (string)brcParams[1];
    widget.ExtraProperties["click_type"] = (string)brcParams[2];
    widget.ExtraProperties["sneaking"] = (bool)brcParams[3];
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.RightClick.Area.y;
      area.pos.x = lastPos.x + pieces.RightClick.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.RightClick.Filter.y;
        filter.pos.x = lastPos.x + pieces.RightClick.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class DigExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first dig argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second dig argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third dig argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:dig", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    object[] digParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "dig", [(typeof(string), "closest"), (typeof(bool), false), (typeof(float), 1), (typeof(bool), false), (typeof(string), "up")]);
    string placeOrderValue = (string)digParams[0];
    bool useMaxActionsValue = (bool)digParams[1];
    float maxActionsFloat = (float)digParams[2];
    int maxActions = (int)maxActionsFloat;
    bool requireToolValue = (bool)digParams[3];
    string side = (string)digParams[4];
    widget.ExtraProperties["dig_place"] = new 
    {
      order = placeOrderValue,
      use_max_actions = useMaxActionsValue,
      max_actions = maxActions
    };
    widget.ExtraProperties["require_hoe"] = requireToolValue;
    widget.ExtraProperties["dig_side"] = side;
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.Dig.Area.y;
      area.pos.x = lastPos.x + pieces.Dig.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.Dig.Filter.y;
        filter.pos.x = lastPos.x + pieces.Dig.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class HarvestExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first harvest argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second harvest argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third harvest argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:harvest", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    object[] digParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "harvest", [(typeof(string), "closest"), (typeof(bool), false), (typeof(float), 1), (typeof(bool), false)]);
    string placeOrderValue = (string)digParams[0];
    bool useMaxActionsValue = (bool)digParams[1];
    float maxActionsFloat = (float)digParams[2];
    int maxActions = (int)maxActionsFloat;
    bool requireToolValue = (bool)digParams[3];
    widget.ExtraProperties["dig_place"] = new 
    {
      order = placeOrderValue,
      use_max_actions = useMaxActionsValue,
      max_actions = maxActions
    };
    widget.ExtraProperties["require_hoe"] = requireToolValue;
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.Dig.Area.y;
      area.pos.x = lastPos.x + pieces.Dig.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.Dig.Filter.y;
        filter.pos.x = lastPos.x + pieces.Dig.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class PlaceExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first place argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second place argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third place argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:place", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    object[] digParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "place", [(typeof(string), "closest"), (typeof(bool), false), (typeof(float), 1f), (typeof(bool), false)]);
    string placeOrderValue = (string)digParams[0];
    bool useMaxActionsValue = (bool)digParams[1];
    float maxActionsFloat = (float)digParams[2];
    int maxActions = (int)maxActionsFloat;
    bool randomize = (bool)digParams[3];
    widget.ExtraProperties["dig_place"] = new 
    {
      order = placeOrderValue,
      use_max_actions = useMaxActionsValue,
      max_actions = maxActions
    };
    widget.ExtraProperties["randomize"] = randomize;
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.Dig.Area.y;
      area.pos.x = lastPos.x + pieces.Dig.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.Dig.Filter.y;
        filter.pos.x = lastPos.x + pieces.Dig.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class PrimaryEntityExterns 
{
  public static Widget[] process(string name, pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement areas = @extern.parameters[0];
    statement? filters = @extern.parameters.Length > 1 ? @extern.parameters[1] : null;
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for first entity right click argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected string array for second entity right click argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:{name}", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.RightClickEntity.Area.y;
      area.pos.x = lastPos.x + pieces.RightClickEntity.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Expression ext in filterArr.expressions.Cast<statement.Expression>()) 
      {
        currentIndex++;
        expression expr = ext.expression;
        if (expr is not expression.Literal literal) throw new VisitorError($"Expected literal for entity name filter, got {expr.GetType().Name}", ext);
        if (literal.value is not string entityName) throw new VisitorError($"Expected string for entity name filter, got {literal.value.GetType().Name}", ext);
        Widget text = new("pneumaticcraft:text", new() { y = lastPos.y + lastSize.y + pieces.RightClickEntity.Text.y, x = lastPos.x + pieces.RightClickEntity.Text.x * currentIndex });
        text.ExtraProperties["string"] = entityName;
        allWidgets = [..allWidgets, text];
      }
    }
    return allWidgets;
  }
}

static class ItemPickupExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first pickup item argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second pickup item argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third pickup item argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:pickup_item", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    object[] digParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "pickup item", [(typeof(bool), false)]);
    bool canSteal = (bool)digParams[0];
    widget.ExtraProperties["can_steal"] = canSteal;
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.PickUpItems.Area.y;
      area.pos.x = lastPos.x + pieces.PickUpItems.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.PickUpItems.Filter.y;
        filter.pos.x = lastPos.x + pieces.PickUpItems.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class ItemDropExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first drop item argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second drop item argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected filter array for third drop item argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:drop_item", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    object[] digParams = VisitorUtil.GetLiteralParams(paramArr.expressions, "drop item", [(typeof(bool), false), (typeof(float), 1f), (typeof(bool), false), (typeof(bool), false)]);
    bool use_count = (bool)digParams[0];
    float countVal = (float)digParams[1];
    int count = (int)countVal;
    bool drop_straight = (bool)digParams[2];
    bool pick_delay = (bool)digParams[3];
    widget.ExtraProperties["inv"] = new 
    {
      use_count,
      count
    };
    widget.ExtraProperties["drop_straight"] = drop_straight;
    widget.ExtraProperties["pick_delay"] = pick_delay;
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.DropItems.Area.y;
      area.pos.x = lastPos.x + pieces.DropItems.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.DropItems.Filter.y;
        filter.pos.x = lastPos.x + pieces.DropItems.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class InventoryExtern 
{
    public static Widget[] process(string name, pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first {name} argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second {name} argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected item filter array for third {name} argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:{(name == "export_inventory" ? "inventory_export" : "inventory_import")}", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    if (paramArr.expressions[0] is not statement.Array sideArray) 
    {
      throw new VisitorError($"Expected side array for first option of {name} parameters, got {paramArr.expressions[0].GetType().Name}", paramArr.expressions[0]);
    }
    statement? opts = paramArr.expressions.Length > 1 ? paramArr.expressions[1] : null;
    string[] sides = [];
    foreach (statement statement in sideArray.expressions) 
    {
      if (statement is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statement.GetType().Name}", statement);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statement);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statement);
      }
      sides = [..sides, side];
    }
    int sidesInt = AstProcessor.turnSidesIntoInt(sides);
    if (opts != null) 
    {
      if (opts is not statement.Array optArray) 
      {
        throw new VisitorError($"Expected option array, got {opts.GetType().Name}", opts);
      }
      object[] invParams = VisitorUtil.GetLiteralParams(optArray.expressions, name, [(typeof(bool), false), (typeof(float), 1)]);
      bool use_count = (bool)invParams[0];
      float countVal = (float)invParams[1];
      int count = (int)countVal;
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt,
        use_count,
        count
      };
    }
    else 
    {
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt
      };
    }
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ExportToInventory.Area.y;
      area.pos.x = lastPos.x + pieces.ExportToInventory.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = ItemFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.ExportToInventory.Filter.y;
        filter.pos.x = lastPos.x + pieces.ExportToInventory.Filter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class FluidImportExtern 
{
  
    public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first fluid import argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second fluid import argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected item filter array for third fluid import argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:liquid_import", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    if (paramArr.expressions[0] is not statement.Array sideArray) 
    {
      throw new VisitorError($"Expected side array for first option of fluid import parameters, got {paramArr.expressions[0].GetType().Name}", paramArr.expressions[0]);
    }
    statement? opts = paramArr.expressions.Length > 1 ? paramArr.expressions[1] : null;
    string[] sides = [];
    foreach (statement statement in sideArray.expressions) 
    {
      if (statement is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statement.GetType().Name}", statement);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statement);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statement);
      }
      sides = [..sides, side];
    }
    int sidesInt = AstProcessor.turnSidesIntoInt(sides);
    if (opts != null) 
    {
      if (opts is not statement.Array optArray) 
      {
        throw new VisitorError($"Expected option array, got {opts.GetType().Name}", opts);
      }
      object[] invParams = VisitorUtil.GetLiteralParams(optArray.expressions, "fluid import", [(typeof(bool), false), (typeof(float), 1), (typeof(string), "closest"), (typeof(bool), false)]);
      bool use_count = (bool)invParams[0];
      float countVal = (float)invParams[1];
      int count = (int)countVal;
      string order = (string)invParams[2];
      bool voidExcess = (bool)invParams[3];
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt,
        use_count,
        count
      };
      widget.ExtraProperties["order"] = order;
      widget.ExtraProperties["void_excess"] = voidExcess;
    }
    else 
    {
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt
      };
    }
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ImportFluid.Area.y;
      area.pos.x = lastPos.x + pieces.ImportFluid.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = FluidFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.ImportFluid.FluidFilter.y;
        filter.pos.x = lastPos.x + pieces.ImportFluid.FluidFilter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class FluidExportExtern 
{
  
    public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement areas = @extern.parameters[1];
    statement? filters = @extern.parameters.Length > 2 ? @extern.parameters[2] : null;
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected option array for first fluid export argument, got {parameters.GetType().Name}", parameters);
    }
    if (areas is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second fluid export argument, got {areas.GetType().Name}", areas);
    }
    if (filters is not null and not statement.Array) 
    {
      throw new VisitorError($"Expected item filter array for third fluid export argument, got {filters.GetType().Name}", filters);
    }
    Widget widget = new($"pneumaticcraft:liquid_export", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    if (paramArr.expressions[0] is not statement.Array sideArray) 
    {
      throw new VisitorError($"Expected side array for first option of fluid export parameters, got {paramArr.expressions[0].GetType().Name}", paramArr.expressions[0]);
    }
    statement? opts = paramArr.expressions.Length > 1 ? paramArr.expressions[1] : null;
    string[] sides = [];
    foreach (statement statement in sideArray.expressions) 
    {
      if (statement is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statement.GetType().Name}", statement);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statement);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statement);
      }
      sides = [..sides, side];
    }
    int sidesInt = AstProcessor.turnSidesIntoInt(sides);
    if (opts != null) 
    {
      if (opts is not statement.Array optArray) 
      {
        throw new VisitorError($"Expected option array, got {opts.GetType().Name}", opts);
      }
      object[] invParams = VisitorUtil.GetLiteralParams(optArray.expressions, "fluid import", [(typeof(bool), false), (typeof(float), 1), (typeof(bool), false)]);
      bool use_count = (bool)invParams[0];
      float countVal = (float)invParams[1];
      int count = (int)countVal;
      bool voidExcess = (bool)invParams[2];
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt,
        use_count,
        count
      };
      widget.ExtraProperties["place_fluid_blocks"] = voidExcess;
    }
    else 
    {
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt
      };
    }
    statement.Array? filterArr = (statement.Array?)filters;
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ExportFluid.Area.y;
      area.pos.x = lastPos.x + pieces.ExportFluid.Area.x * currentIndex;
      allWidgets = [..allWidgets, area];
    }
    if (filterArr != null) 
    {
      currentIndex = 0;
      foreach (statement.Extern ext in filterArr.expressions.Cast<statement.Extern>()) 
      {
        currentIndex++;
        Widget filter = FluidFilterExtern.process(lastPos, lastSize, ext);
        filter.pos.y = lastPos.y + lastSize.y + pieces.ExportFluid.FluidFilter.y;
        filter.pos.x = lastPos.x + pieces.ExportFluid.FluidFilter.x * currentIndex;
        allWidgets = [..allWidgets, filter];
      }
    }
    return allWidgets;
  }
}

static class RFExterns 
{
  
    public static Widget[] process(string name, pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement parameters = @extern.parameters[0];
    statement area = @extern.parameters[1];
    if (parameters is not statement.Array paramArr) 
    {
      throw new VisitorError($"Expected side array for first {name} argument, got {parameters.GetType().Name}", parameters);
    }
    if (area is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for second {name} argument, got {area.GetType().Name}", area);
    }
    Widget widget = new($"pneumaticcraft:{(name == "export_rf" ? "rf_export" : "rf_import")}", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    if (paramArr.expressions[0] is not statement.Array sideArray) 
    {
      throw new VisitorError($"Expected side array for first option of {name} parameters, got {paramArr.expressions[0].GetType().Name}", paramArr.expressions[0]);
    }
    statement? opts = paramArr.expressions.Length > 1 ? paramArr.expressions[1] : null;
    string[] sides = [];
    foreach (statement statement in sideArray.expressions) 
    {
      if (statement is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statement.GetType().Name}", statement);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statement);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statement);
      }
      sides = [..sides, side];
    }
    int sidesInt = AstProcessor.turnSidesIntoInt(sides);
    if (opts != null) 
    {
      if (opts is not statement.Array optArray) 
      {
        throw new VisitorError($"Expected option array, got {opts.GetType().Name}", opts);
      }
      object[] invParams = VisitorUtil.GetLiteralParams(optArray.expressions, "fluid import", [(typeof(bool), false), (typeof(float), 1)]);
      bool use_count = (bool)invParams[0];
      float countVal = (float)invParams[1];
      int count = (int)countVal;
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt,
        use_count,
        count
      };
    }
    else 
    {
      widget.ExtraProperties["inv"] = new
      {
        sides = sidesInt
      };
    }
    int currentIndex = 0;
    foreach (statement.Extern ext in areaArr.expressions.Cast<statement.Extern>()) 
    {
      currentIndex++;
      Widget areaW = AreaExternVisitor.process(lastPos, lastSize, ext.parameters);
      areaW.pos.y = lastPos.y + lastSize.y + pieces.ExportRf.Area.y;
      areaW.pos.x = lastPos.x + pieces.ExportRf.Area.x * currentIndex;
      allWidgets = [..allWidgets, areaW];
    }
    return allWidgets;
  }
}

static class EmitRedstoneExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    Widget widget = new($"pneumaticcraft:emit_redstone", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    statement firstParam = @extern.parameters[0];
    statement redstoneLevel = @extern.parameters[1];
    if (firstParam is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {firstParam.GetType().Name}", firstParam);
    }
    statement.Expression redstoneLevelExpr = (statement.Expression)((redstoneLevel is statement.Expression) ? redstoneLevel : throw new VisitorError($"Expected expression for emit_redstone parameter, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]));
    float redstoneLvl = redstoneLevelExpr.expression is expression.Literal { value: float intVal }
          ? intVal
          : throw new VisitorError($"Expected integer literal for emit redstone parameter, got {redstoneLevelExpr.expression.GetType().Name}", redstoneLevelExpr);
    Widget textWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.EmitRedstone.Area.x, y = lastPos.y + lastSize.y });
    textWidget.ExtraProperties["string"] = $"{(int)redstoneLvl}";
    string[] sides = [];
    foreach (statement statement in sidesArr.expressions) 
    {
      if (statement is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statement.GetType().Name}", statement);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statement);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statement);
      }
      sides = [..sides, side];
    }
    int sidesInt = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["sides"] = sidesInt;
    return [widget, textWidget];
  }
}

static class ExternalProgramExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement firstParam = @extern.parameters[0];
    if (firstParam is not statement.Array areasArr) 
    {
      throw new VisitorError($"Expected array of areas, got {firstParam.GetType().Name}", firstParam);
    }
    statement.Expression? doneWhenDepart = 
      @extern.parameters.Length > 1
      ? ((@extern.parameters[1] is statement.Expression) ? @extern.parameters[1] as statement.Expression : throw new VisitorError($"Expected expression for externalProgram parameter, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1])) : null;
    bool doneWhenDepartVal = doneWhenDepart is not null && (doneWhenDepart.expression is expression.Literal { value: bool intVal }
          ? intVal
          : throw new VisitorError($"Expected integer literal for externalProgram parameter, got {doneWhenDepart.expression.GetType().Name}", doneWhenDepart));
    Widget widget = new($"pneumaticcraft:external_program", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    widget.ExtraProperties["share_variables"] = doneWhenDepartVal;
    Widget[] areas = [];
    int posMult = 0;
    foreach (statement.Extern statements in areasArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y;
      area.pos.x = lastPos.x + pieces.GoToLocation.Area.x * posMult;
      areas = [..areas, area];
    }
    return [widget, ..areas];
  }
}
static class LogisticsExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    statement firstParam = @extern.parameters[0];
    if (firstParam is not statement.Array areasArr) 
    {
      throw new VisitorError($"Expected array of areas, got {firstParam.GetType().Name}", firstParam);
    }
    Widget widget = new($"pneumaticcraft:logistics", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] areas = [];
    int posMult = 0;
    foreach (statement.Extern statements in areasArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y;
      area.pos.x = lastPos.x + pieces.Logistics.Area.x * posMult;
      areas = [..areas, area];
    }
    return [widget, ..areas];
  }
}

static class WaitExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    Widget widget = new($"pneumaticcraft:wait", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    statement firstParam = @extern.parameters[0];
    statement.Expression redstoneLevelExpr = (statement.Expression)((firstParam is statement.Expression) ? firstParam : throw new VisitorError($"Expected expression for emit_redstone parameter, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]));
    float redstoneLvl = redstoneLevelExpr.expression is expression.Literal { value: float intVal }
          ? intVal
          : throw new VisitorError($"Expected integer literal for wait time, got {redstoneLevelExpr.expression.GetType().Name}", redstoneLevelExpr);
    Widget textWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.EmitRedstone.Area.x, y = lastPos.y + lastSize.y });
    textWidget.ExtraProperties["string"] = $"{(int)redstoneLvl}";
    return [widget, textWidget];
  }
}

static class ForEachCoordinateExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array areaArr) 
    {
      throw new VisitorError($"Expected area array for first foreach param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (@extern.parameters[1] is not statement.Expression label) {
      throw new VisitorError($"Expected expression for second foreach param, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression varExpr) 
    {
      throw new VisitorError($"Expected expression for third foreach param, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string jumpLabel = label.expression is expression.Literal jumpExpr ?
        jumpExpr.value is string labelStr ?
          labelStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpExpr.value.GetType().Name}", label)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${label.expression.GetType().Name}", label);
    string varName = varExpr.expression is expression.Literal variableExpr ?
        variableExpr.value is string varStr ?
          varStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${variableExpr.value.GetType().Name}", varExpr)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${varExpr.expression.GetType().Name}", varExpr);
    Widget widget = new("pneumaticcraft:for_each_coordinate", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    widget.ExtraProperties["var"] = varName;
    Widget textlabel = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ForEachCoordinate.Text.x, y = lastPos.y + lastSize.y + pieces.ForEachCoordinate.Text.y });
    textlabel.ExtraProperties["string"] = jumpLabel;
    Widget[] allWidgets = [widget, textlabel];
    int posMult = 0;
    foreach (statement.Extern statements in areaArr.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y;
      area.pos.x = lastPos.x + pieces.ForEachCoordinate.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    return allWidgets;
  }
}

static class CoordinateConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {@extern.parameters[0].GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array firstCoords) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array secondCoords) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    Widget widget = new("pneumaticcraft:condition_coordinate", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    statement[] remaining = [..conditionParams.expressions.Skip(1)];
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    widget.ExtraProperties["cond_op"] = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le";
    string[] axes = [];
    foreach (statement remainder in remaining) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string axis) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      axes = [..axes, axis];
    }
    int axisCount = AstProcessor.turnAxesIntoInt(axes);
    if (axisCount == 0) throw new VisitorError($"Atleast one axis is required for coordinate conditions to function!", conditionParams);
    widget.ExtraProperties["axis_options"] = new 
    {
      axes = axisCount
    };
    Widget[] allWidgets = [widget];
    foreach (statement stmt in firstCoords.expressions) 
    {
      if (stmt is not statement.Extern externCall) 
      {
        throw new VisitorError($"Expected extern call to coordinate() in coordinates array, got {stmt.GetType().Name}", stmt);
      }
      if (externCall.name.lexeme != "coordinate") {
        throw new VisitorError($"Expected extern call to coordinate() in coordinates array, got call to {externCall.name.lexeme}", stmt);
      }
      Widget coordinateWidget = CoordinateExternVisitor.process(lastPos, lastSize, externCall);
      coordinateWidget.pos.x = lastPos.x + pieces.ConditionCoordinate.Coordinate1.x;
      coordinateWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionCoordinate.Coordinate1.y;
      allWidgets = [..allWidgets, coordinateWidget];
    }
    foreach (statement stmt in secondCoords.expressions) 
    {
      if (stmt is not statement.Extern externCall) 
      {
        throw new VisitorError($"Expected extern call to coordinate() in coordinates array, got {stmt.GetType().Name}", stmt);
      }
      if (externCall.name.lexeme != "coordinate") {
        throw new VisitorError($"Expected extern call to coordinate() in coordinates array, got call to {externCall.name.lexeme}", stmt);
      }
      Widget coordinateWidget = CoordinateExternVisitor.process(lastPos, lastSize, externCall);
      coordinateWidget.pos.x = lastPos.x + pieces.ConditionCoordinate.Coordinate2.x;
      coordinateWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionCoordinate.Coordinate2.y;
      allWidgets = [..allWidgets, coordinateWidget];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionCoordinate.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionCoordinate.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionCoordinate.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionCoordinate.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class RedstoneConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_redstone", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is int measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionRedstone.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class LightConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_light", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionRedstone.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class ItemConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of item filters, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    if (@extern.parameters[4] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 5 ?
      @extern.parameters[5] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[5].GetType().Name}", @extern.parameters[5])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_item_inventory", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern filter in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filterWidget = ItemFilterExtern.process(lastPos, lastSize, filter);
      filterWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Filter.y;
      filterWidget.pos.x = lastPos.x + pieces.ConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, filterWidget];
    }
    posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionItems.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class BlockConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (@extern.parameters[1] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of item filters, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 5 ?
      @extern.parameters[5] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[5].GetType().Name}", @extern.parameters[5])
      : null;
    Widget widget = new("pneumaticcraft:condition_block", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    bool check_air = false;
    bool check_liquid = false;
    string? measure_var = null;
    if (conditionParams.expressions.Length > 0 && conditionParams.expressions[0] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression check_air_expr) 
    {
      bool andFunc = (check_air_expr.expression is not null && check_air_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool, got {andFuncLiteral.value.GetType().Name}", check_air_expr)
        : throw new VisitorError($"Expected boolean literal, got {check_air_expr.expression?.GetType().Name ?? "null"}", check_air_expr);
      check_air = andFunc;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression check_liquid_expr) 
    {
      bool andFunc = (check_liquid_expr.expression is not null && check_liquid_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool, got {andFuncLiteral.value.GetType().Name}", check_liquid_expr)
        : throw new VisitorError($"Expected boolean literal, got {check_liquid_expr.expression?.GetType().Name ?? "null"}", check_liquid_expr);
      check_liquid = andFunc;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        and_func
      };
    }
    widget.ExtraProperties["inv"] = new {};
    widget.ExtraProperties["check_air"] = check_air;
    widget.ExtraProperties["check_liquid"] = check_liquid;
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern filter in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filterWidget = ItemFilterExtern.process(lastPos, lastSize, filter);
      filterWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Filter.y;
      filterWidget.pos.x = lastPos.x + pieces.ConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, filterWidget];
    }
    posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionItems.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class FluidConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of fluid filters, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    if (@extern.parameters[4] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 5 ?
      @extern.parameters[5] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[5].GetType().Name}", @extern.parameters[5])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_liquid_inventory", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern filter in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filterWidget = FluidFilterExtern.process(lastPos, lastSize, filter);
      filterWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Filter.y;
      filterWidget.pos.x = lastPos.x + pieces.ConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, filterWidget];
    }
    posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionItems.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionItems.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class EntityConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of entity names, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_entity", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le"
      };
    }
    widget.ExtraProperties["inv"] = new 
    {
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Expression filter in filters.expressions.Cast<statement.Expression>()) 
    {
      if (filter is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {filter.GetType().Name}", filter);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", filter);
      }
      if (literal.value is not string entity) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", filter);
      }
      posMult++;
      Widget textWidget = new($"pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionEntity.Text1.x * posMult, y = lastPos.y + lastSize.y + pieces.ConditionEntity.Text1.y });
      textWidget.ExtraProperties["string"] = entity;
      allWidgets = [..allWidgets, textWidget];
    }
    posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionEntity.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionEntity.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionEntity.Text2.x, y = lastPos.y + lastSize.y + pieces.ConditionEntity.Text2.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionEntity.Text2.x, y = lastPos.y + lastSize.y + pieces.ConditionEntity.Text2.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class PressureConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_pressure", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is int measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionPressure.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionPressure.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionPressure.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionPressure.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class RFConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array sidesArr) 
    {
      throw new VisitorError($"Expected array of sides, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Array areas) 
    {
      throw new VisitorError($"Expected array of coords, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    if (@extern.parameters[3] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 4 ?
      @extern.parameters[4] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[4].GetType().Name}", @extern.parameters[4])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:condition_rf", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    bool and_func = false;
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression and_func_expr) 
    {
      bool andFunc = (and_func_expr.expression is not null && and_func_expr.expression is expression.Literal andFuncLiteral) ? 
        andFuncLiteral.value is bool andFuncVal ? andFuncVal : throw new VisitorError($"Expected bool for compIsAnd, got {andFuncLiteral.value.GetType().Name}", and_func_expr)
        : throw new VisitorError($"Expected boolean literal for and condition, got {and_func_expr.expression?.GetType().Name ?? "null"}", and_func_expr);
      and_func = andFunc;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 3 && conditionParams.expressions[3] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is int measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func,
        measure_var
      };
    }
    else 
    {
      widget.ExtraProperties["cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        and_func
      };
    }
    string[] sides = [];
    foreach (statement remainder in sidesArr.expressions) 
    {
      if (remainder is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {remainder.GetType().Name}", remainder);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", remainder);
      }
      if (literal.value is not string side) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", remainder);
      }
      sides = [..sides, side];
    }
    int sideCount = AstProcessor.turnSidesIntoInt(sides);
    widget.ExtraProperties["inv"] = new 
    {
      sides = sideCount,
      count
    };
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in areas.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = AreaExternVisitor.process(lastPos, lastSize, statements.parameters);
      area.pos.y = lastPos.y + lastSize.y + pieces.ConditionPressure.Area.y;
      area.pos.x = lastPos.x + pieces.ConditionPressure.Area.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionPressure.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionPressure.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class ItemFilterConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Extern filter) 
    {
      throw new VisitorError($"Expected item filter, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (@extern.parameters[1] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected arrays of item filters, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 3 ?
      @extern.parameters[3] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3])
      : null;
    Widget widget = new("pneumaticcraft:condition_item", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    Widget[] allWidgets = [widget];
    statement.Array[] filts = [..filters.expressions.Cast<statement.Array>()];
    Widget checkedWidget = ItemFilterExtern.process(lastPos, lastSize, filter);
    checkedWidget.pos.x = lastPos.x + pieces.ConditionBlock.Area.x;
    checkedWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionBlock.Area.y;
    allWidgets = [..allWidgets, checkedWidget];
    statement.Array righthandFilters = filts[0];
    statement.Array? lefthandFilters = filts.Length > 1 ? filts[1] : null;
    int posMult = 0;
    foreach (statement.Extern righthandFilt in righthandFilters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget filterWidget = ItemFilterExtern.process(lastPos, lastSize, righthandFilt);
      filterWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionBlock.Filter.y;
      filterWidget.pos.x = lastPos.x + pieces.ConditionBlock.Filter.x * posMult;
      allWidgets = [..allWidgets, filterWidget];
    }
    if (lefthandFilters != null) 
    {
      posMult = 0;
      foreach (statement.Extern lefthandFilt in lefthandFilters.expressions.Cast<statement.Extern>()) 
      {
        posMult++;
        Widget filterWidget = ItemFilterExtern.process(lastPos, lastSize, lefthandFilt);
        filterWidget.pos.y = lastPos.y + lastSize.y + pieces.ConditionBlock.Filter.y;
        filterWidget.pos.x = lastPos.x - pieces.ConditionBlock.Filter.x * posMult;
        allWidgets = [..allWidgets, filterWidget];
      }
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionItems.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DroneItemConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of item filters, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 3 ?
      @extern.parameters[3] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_item", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = ItemFilterExtern.process(lastPos, lastSize, statements);
      area.pos.y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Filter.y;
      area.pos.x = lastPos.x + pieces.DroneConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DroneFluidConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of fluid filters, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 3 ?
      @extern.parameters[3] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_liquid", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = FluidFilterExtern.process(lastPos, lastSize, statements);
      area.pos.y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Filter.y;
      area.pos.x = lastPos.x + pieces.DroneConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DroneEntityConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of entity names, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 3 ?
      @extern.parameters[3] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_entity", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Expression statements in filters.expressions.Cast<statement.Expression>()) 
    {
      if (statements is not statement.Expression expr) 
      {
        throw new VisitorError($"Expected expression, got {statements.GetType().Name}", statements);
      }
      if (expr.expression is not expression.Literal literal) 
      {
        throw new VisitorError($"Expected literal, got {expr.expression.GetType().Name}", statements);
      }
      if (literal.value is not string entityName) 
      {
        throw new VisitorError($"Expected string, got {literal.value.GetType().Name}", statements);
      }
      posMult++;
      Widget textWidget = new($"pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionEntity.Text1.x * posMult, y = lastPos.y + lastSize.y + pieces.DroneConditionEntity.Text1.y });
      textWidget.ExtraProperties["string"] = entityName;
      allWidgets = [..allWidgets, textWidget];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionEntity.Text2.x, y = lastPos.y + lastSize.y + pieces.DroneConditionEntity.Text2.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.DroneConditionEntity.Text2.x, y = lastPos.y + lastSize.y + pieces.DroneConditionEntity.Text2.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DronePressureConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 2 ?
      @extern.parameters[2] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_pressure", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionPressure.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.DroneConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionPressure.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DroneRFConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 2 ?
      @extern.parameters[2] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_rf", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionPressure.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.DroneConditionPressure.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionPressure.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}

static class DroneUpgradeConditionExtern 
{
  public static Widget[] process(pieces.Coordinate lastPos, pieces.Coordinate lastSize, statement.Extern @extern) 
  {
    if (@extern.parameters[0] is not statement.Array conditionParams) 
    {
      throw new VisitorError($"Expected array for first condition param, got {@extern.parameters[0].GetType().Name}", @extern.parameters[0]);
    }
    if (conditionParams.expressions[0] is not statement.Expression cond_op_expr) 
    {
      throw new VisitorError($"Expected condition operator expression, got {conditionParams.GetType().Name}", conditionParams);
    }
    if (@extern.parameters[1] is not statement.Array filters) 
    {
      throw new VisitorError($"Expected array of item filters, got {@extern.parameters[1].GetType().Name}", @extern.parameters[1]);
    }
    if (@extern.parameters[2] is not statement.Expression jumpLabel) 
    {
      throw new VisitorError($"Expected label name to jump to if truthy, got {@extern.parameters[2].GetType().Name}", @extern.parameters[2]);
    }
    string truthyLabel = jumpLabel.expression is expression.Literal truthyLiteral ?
        truthyLiteral.value is string truthyStr ?
          truthyStr
        : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${truthyLiteral.value.GetType().Name}", jumpLabel)
      : throw new VisitorError($"Expected string literal for label to jump to if truthy, got ${jumpLabel.expression.GetType().Name}", jumpLabel);
    string? falsyLabel = @extern.parameters.Length > 3 ?
      @extern.parameters[3] is statement.Expression secondExpr ?
          secondExpr.expression is expression.Literal labelLiteral ?
            labelLiteral.value is string labelStr ?
              labelStr
            : throw new VisitorError($"Expected string literal for label to jump to if falsy, got ${labelLiteral.value.GetType().Name}", secondExpr)
          : throw new VisitorError($"Expected literal for label to jump to if falsy, got {secondExpr.expression.GetType().Name}", secondExpr)
        : throw new VisitorError($"Expected expression for label to jump to if falsy, got {@extern.parameters[3].GetType().Name}", @extern.parameters[3])
      : null;
    string conditionOp = (cond_op_expr.expression is not null && cond_op_expr.expression is expression.Literal condLiteral) ? 
      condLiteral.value is string conditionOperator ? conditionOperator : throw new VisitorError($"Expected string for condition operator, got {condLiteral.value.GetType().Name}", cond_op_expr)
      : throw new VisitorError($"Expected string literal for condition operator, got {cond_op_expr.expression?.GetType().Name ?? "null"}", cond_op_expr);
    Widget widget = new("pneumaticcraft:drone_condition_upgrades", new() { x = lastPos.x, y = lastPos.y + lastSize.y });
    string? measure_var = null;
    int count = 1;
    if (conditionParams.expressions.Length > 1 && conditionParams.expressions[1] is statement.Expression measure_var_expr) 
    {
      string measureVar = (measure_var_expr.expression is not null && measure_var_expr.expression is expression.Literal measureLiteral) ? 
        measureLiteral.value is string measureVarVal ? measureVarVal : throw new VisitorError($"Expected string for measure variable, got {measureLiteral.value.GetType().Name}", measure_var_expr)
        : throw new VisitorError($"Expected string literal for measure variable, got {measure_var_expr.expression?.GetType().Name ?? "null"}", measure_var_expr);
      measure_var = measureVar;
    }
    if (conditionParams.expressions.Length > 2 && conditionParams.expressions[2] is statement.Expression count_expr) 
    {
      int countVar = (count_expr.expression is not null && count_expr.expression is expression.Literal countLiteral) ? 
        countLiteral.value is float measureVarVal ? (int)measureVarVal : throw new VisitorError($"Expected int for count, got {countLiteral.value.GetType().Name}", count_expr)
        : throw new VisitorError($"Expected int literal for count, got {count_expr.expression?.GetType().Name ?? "null"}", count_expr);
      count = countVar;
    }
    if (measure_var != null) 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        measure_var,
        required_count = count
      };
    }
    else 
    {
      widget.ExtraProperties["drone_cond"] = new 
      {
        cond_op = conditionOp == "==" ? "eq" : conditionOp == ">=" ? "ge" : "le",
        required_count = count
      };
    }
    Widget[] allWidgets = [widget];
    int posMult = 0;
    foreach (statement.Extern statements in filters.expressions.Cast<statement.Extern>()) 
    {
      posMult++;
      Widget area = ItemFilterExtern.process(lastPos, lastSize, statements);
      area.pos.y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Filter.y;
      area.pos.x = lastPos.x + pieces.DroneConditionItems.Filter.x * posMult;
      allWidgets = [..allWidgets, area];
    }
    Widget jumpTruthyWidget = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.DroneConditionItems.Text.x, y = lastPos.y + lastSize.y + pieces.DroneConditionItems.Text.y });
    jumpTruthyWidget.ExtraProperties["string"] = truthyLabel;
    allWidgets = [..allWidgets, jumpTruthyWidget];
    if (falsyLabel != null) 
    {
      Widget jumpFalsyWidget = new("pneumaticcraft:text", new() { x = lastPos.x - pieces.ConditionRedstone.Text.x, y = lastPos.y + lastSize.y + pieces.ConditionRedstone.Text.y });
      jumpFalsyWidget.ExtraProperties["string"] = falsyLabel;
      allWidgets = [..allWidgets, jumpFalsyWidget];
    }
    return allWidgets;
  }
}