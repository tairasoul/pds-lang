using tairasoul.pdsl.extensions;
using statements = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.visitors.returnTypes;
using tairasoul.pdsl.pieces;
using statementVisitors = tairasoul.pdsl.visitors.statements;
using tairasoul.pdsl.visitors.errors;

namespace tairasoul.pdsl.processor;

class LastStatement 
{
  public int x;
  public int y;
  public statements? statement;
}

class AstProcessor(statements[] ast)
{
  private readonly statements[] ast = ast;
  internal LastStatement lastStatement = new() { x = 0, y = 0 };
  internal Coordinate[] commentPositions = [];
  internal Coordinate currentFunctionPos = new() 
  {
    x = -1000,
    y = -2000
  };
  internal Coordinate lastSize = new() 
  {
    x = 0,
    y = 0
  };
  
  public event Action<string, statements.Expression> processingError = (reason, statement) => {};
  
  public Widget[] getResult() 
  {
    Widget[] widgets = [];
    Widget commentWidget = new("pneumaticcraft:comment", new Coordinate { x = lastStatement.x, y = lastStatement.y - 20 });
    commentWidget.ExtraProperties["string"] = "Generated with pds-lang v0.1.0";
    widgets = [..widgets, commentWidget];
    foreach (statements statement in ast) 
    {
      try {
        VisitorReturn? processed = processStatement(statement);
        if (processed == null) continue;
        widgets = [..widgets, ..processed.widgets];
        lastStatement = new() { x = processed.setLast.x, y = processed.setLast.y, statement = statement };
      }
      catch (VisitorError ex) 
      {
        processingError.Invoke(ex.reason, (statements.Expression)ex.statement);
      }
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
      return statementVisitors.ExternVisitor.process(lastPos, ref lastSize, @extern);
    }
    return null;
  }
}