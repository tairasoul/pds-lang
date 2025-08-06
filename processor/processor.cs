using tairasoul.pdsl.extensions;
using statements = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.visitors.returnTypes;
using tairasoul.pdsl.pieces;
using statementVisitors = tairasoul.pdsl.visitors.statements;
using tairasoul.pdsl.visitors.errors;
using tairasoul.pdsl.luapiece;
using MoonSharp.Interpreter;

namespace tairasoul.pdsl.processor;

class LastStatement 
{
  public int x;
  public int y;
  public statements? statement;
}

class AstProcessor(statements[] ast, LuaEnvironment env)
{
  internal LuaEnvironment env = env;
  private readonly statements[] ast = ast;
  internal LastStatement lastStatement = new() { x = 0, y = 0 };
  internal Coordinate[] commentPositions = [];
  internal Coordinate currentFunctionPos = new() 
  {
    x = -8000,
    y = -2000
  };
  internal Coordinate lastSize = new() 
  {
    x = 0,
    y = 0
  };

  internal int BlockCount = 0;

  internal Widget[] additionalWidgets = [];

  public Action<string, statements> processingError = (reason, statement) => {};
  private bool TableIsArray(Table table) 
  {
    bool isArray = true;
    foreach (TablePair pair in table.Pairs) 
    {
      if (pair.Key.Type != DataType.Number || (pair.Key.Type == DataType.String && pair.Key.CastToNumber() == null) || (pair.Value.Type == DataType.String && pair.Value.String == "__EMPTY_TABLE"))
        isArray = false;
    }
    return isArray;
  }
  
  private object? DynToValidKey(DynValue value) 
  {
    switch (value.Type) 
    {
      case DataType.Number:
        return value.Number;
      case DataType.Boolean:
        return value.Boolean;
      case DataType.String:
        return value.String;
    }
    return null;
  }
  
  private void ProcessSubtable(Table table, ref Dictionary<object, object> objs) 
  {
    Dictionary<object, object> dict = [];
    foreach (TablePair pair in table.Pairs) 
    {
      object? key = DynToValidKey(pair.Key);
      if (key == null) continue;
      DynValue dyn = pair.Value;
      switch (dyn.Type) 
      {
        case DataType.String:
          string val = dyn.String;
          if (val != "__EMPTY_TABLE")
            objs[key] = val;
          break;
        case DataType.Boolean:
        case DataType.Number:
          objs[key] = DynToValidKey(dyn);
          break;
        case DataType.Table:
          Table t = dyn.Table;
          if (TableIsArray(t)) 
          {
            object[] objs1 = [];
            ProcessSubtable(t, ref objs1);
            dict.Add(key, objs1);
          }
          else 
          {
            Dictionary<object, object> thing = [];
            ProcessSubtable(t, ref thing);
            objs[key] = thing;
          }
          break;
      }
    }
  }
  
  private void ProcessSubtable(Table table, ref object[] objs) 
  {
    foreach (TablePair pair in table.Pairs) 
    {
      DynValue dyn = pair.Value;
      switch (dyn.Type) 
      {
        case DataType.Boolean:
        case DataType.Number:
        case DataType.String:
          objs = [..objs, DynToValidKey(dyn)];
          break;
        case DataType.Table:
          Table t = dyn.Table;
          if (TableIsArray(t)) 
          {
            object[] objs1 = [];
            ProcessSubtable(t, ref objs1);
            objs = [.. objs, objs1];
          }
          else 
          {
            Dictionary<object, object> thing = [];
            ProcessSubtable(t, ref thing);
            objs = [.. objs, thing];
          }
          break;
      }
    }
  }
  
  private void ProcessTable(Table table, Widget widget, string key) 
  {
    bool arr = TableIsArray(table);
    if (arr) 
    {
      object[] objs = [];
      ProcessSubtable(table, ref objs);
      widget.ExtraProperties[key] = objs;
    }
    else 
    {
      Dictionary<object, object> dict = [];
      ProcessSubtable(table, ref dict);
      widget.ExtraProperties[key] = dict;
    }
  }
  private void ProcessWidget(Widget widget) { 
    foreach (string key in widget.ExtraProperties.Keys) 
    {
      object extraProp = widget.ExtraProperties[key];
      if (extraProp is DynValue dyn) 
      {
        switch (dyn.Type) 
        {
          case DataType.Boolean:
          case DataType.Number:
          case DataType.String:
            widget.ExtraProperties[key] = DynToValidKey(dyn);
            break;
          case DataType.Table:
            ProcessTable(dyn.Table, widget, key);
            break;
        }
      }
    }
  }
  public Widget[] getResult() 
  {
    Widget[] widgets = [];
    Widget commentWidget = new("pneumaticcraft:comment", new Coordinate { x = lastStatement.x, y = lastStatement.y - 20 });
    commentWidget.ExtraProperties["string"] = "Compiled with pds-lang v0.1.0";
    widgets = [..widgets, commentWidget];
    foreach (statements statement in ast) 
    {
      try {
        VisitorReturn? processed = processStatement(statement);
        if (processed == null) 
        {
          processingError.Invoke($"Failed to process", statement);
          break;
        };
        foreach (Widget widget in processed.widgets) 
        {
          ProcessWidget(widget);
          widgets = [..widgets, widget];
        }
        lastStatement = new() { x = processed.setLast.x, y = processed.setLast.y, statement = statement };
      }
      catch (VisitorError ex) 
      {
        processingError.Invoke(ex.reason, (statements.Expression)ex.statement);
      }
    }
    foreach (Widget widget in additionalWidgets) 
    {
      ProcessWidget(widget);
      widgets = [.. widgets, widget];
    }
    return widgets;
  }
  
  internal static int turnAxesIntoInt(string[] axes) 
  {
    string[] alreadyProcessed = [];
    int axesInt = 0;
    foreach (string axis in axes) 
    {
      if (alreadyProcessed.Includes(axis)) continue;
      alreadyProcessed = [..alreadyProcessed, axis];
      switch (axis.ToLower()) 
      {
        case "x":
          axesInt += 1;
          break;
        case "y":
          axesInt += 2;
          break;
        case "z":
          axesInt += 4;
          break;
      }
    }
    return axesInt;
  }
  
  internal static int turnSidesIntoInt(string[] sides) 
  {
    string[] alreadyProcessed = [];
    int sideCount = 0;
    foreach (string side in sides) 
    {
      if (alreadyProcessed.Includes(side)) continue;
      alreadyProcessed = [..alreadyProcessed, side];
      switch (side.ToLower()) 
      {
        case "down":
          sideCount += 1;
          break;
        case "up":
          sideCount += 2;
          break;
        case "north":
          sideCount += 4;
          break;
        case "south":
          sideCount += 8;
          break;
        case "west":
          sideCount += 16;
          break;
        case "east":
          sideCount += 32;
          break;
      }
    }
    return sideCount;
  }
  
  internal VisitorReturn? processStatement(statements statement, bool inFunction = false) 
  {
    Coordinate lastPos = inFunction ? currentFunctionPos : new Coordinate { x = lastStatement.x, y = lastStatement.y };
    if (statement is statements.Comment comment) 
    {
      return statementVisitors.CommentVisitor.process(lastPos, ref lastSize, comment, ref commentPositions);
    }
    if (statement is statements.FunctionStatement function) 
    {
      return statementVisitors.FunctionStatementVisitor.process(lastPos, ref lastSize, function, this);
    }
    if (statement is statements.Subroutine subroutine) 
    {
      return statementVisitors.SubroutineVisitor.process(lastPos, ref lastSize, subroutine);
    }
    if (statement is statements.Extern @extern) 
    {
      return statementVisitors.ExternVisitor.process(env, lastPos, ref lastSize, @extern, this);
    }
    return null;
  }
}