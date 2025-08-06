using tairasoul.pdsl.visitors.returnTypes;
using expression = tairasoul.pdsl.ast.expressions.Expression;
using statement = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.lexer;
using tairasoul.pdsl.processor;
using tairasoul.pdsl.luapiece;
using MoonSharp.Interpreter;
using tairasoul.pdsl.extensions;
using System.Collections.Generic;
using System.Collections;

namespace tairasoul.pdsl.visitors.statements;

static class CommentVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Comment comment, ref pieces.Coordinate[] CommentPositions) 
  {
    int[] sameY_xValues = [..CommentPositions.Where((coord) => coord.y == lastPos.y - 8 + pieces.Comment.y).Select((coord) => coord.x)];
    bool foundDuplicate = false;
    int currentXOffset = 40;
    int traversed = 2;
    int multForNonDuplicate = 0;
    foreach (int xValue in sameY_xValues) {
      traversed++;
      if (foundDuplicate) 
      {
        multForNonDuplicate++;
      }
      if (xValue == lastPos.x - currentXOffset) 
      {
        foundDuplicate = true;
      }
    }
    multForNonDuplicate = traversed;
    currentXOffset += 40 * multForNonDuplicate;
    CommentPositions = [..CommentPositions, new() { x = lastPos.x - currentXOffset, y = lastPos.y - 8 + pieces.Comment.y }];
    Widget commentWidget = new("pneumaticcraft:comment", new pieces.Coordinate { x = lastPos.x - currentXOffset, y = lastPos.y - 8 + pieces.Comment.y });
    commentWidget.ExtraProperties["string"] = comment.text;
    VisitorReturn ret = new(lastPos, [commentWidget]);
    return ret;
  }
}

static class FunctionStatementVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.FunctionStatement function, AstProcessor processor) 
  {
    Token label = function.identifier;
    statement.Block block = function.block;
    Widget labelWidget = new("pneumaticcraft:label", new pieces.Coordinate { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y });
    Widget labelTextWidget = new("pneumaticcraft:text", new pieces.Coordinate { x = processor.currentFunctionPos.x + pieces.Label.Text.x, y = processor.currentFunctionPos.y });
    labelTextWidget.ExtraProperties["string"] = label.lexeme;
    processor.lastSize = new() { x = pieces.Label.x, y = pieces.Label.y };
    processor.currentFunctionPos = new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y + 11 };
    VisitorReturn[] visitorReturns = [];
    VisitorReturn initialReturn = new(new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y }, [labelWidget, labelTextWidget]);
    visitorReturns = [..visitorReturns, initialReturn];
    foreach (statement statement in block.statements) 
    {
      VisitorReturn? processed = processor.processStatement(statement, true);
      if (processed == null) continue;
      processor.currentFunctionPos.y = processed.setLast.y;
      visitorReturns = [..visitorReturns, processed];
    }
    Widget[][] widgetsMapped = [..visitorReturns.Select((visitorReturn) => visitorReturn.widgets)];
    Widget[] returnedWidgets = [];
    foreach (Widget[] mapped in widgetsMapped) 
    {
      returnedWidgets = [..returnedWidgets, ..mapped];
    }
    processor.currentFunctionPos.x += 2000;
    VisitorReturn finalReturn = new(lastPos, returnedWidgets);
    return finalReturn;
  }
}

static class SubroutineVisitor 
{
  public static VisitorReturn process(pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Subroutine subroutine) 
  {
    Widget jumpsub = new("pneumaticcraft:jump_sub", new() { x = lastPos.x, y = lastPos.y });
    Widget label = new("pneumaticcraft:text", new() { x = lastPos.x + pieces.JumpSubroutine.Text.x, y = lastPos.y });
    label.ExtraProperties["string"] = subroutine.label.lexeme;
    VisitorReturn ret = new(new() { x = lastPos.x, y = lastPos.y + 11 }, [jumpsub, label]);
    lastSize = new() { x = pieces.JumpSubroutine.x, y = pieces.JumpSubroutine.y };
    return ret;
  }
}

static class ExternVisitor 
{
  static object[] processArgs(LuaEnvironment luaEnv, statement[] parameters, AstProcessor processor) 
  {
    object[] args = [];
    foreach (statement stmt in parameters) 
    {
      if (stmt is statement.Array arr) 
      {
        args = [.. args, processArgs(luaEnv, arr.expressions, processor)];
        continue;
      }
      if (stmt is statement.Expression expres)
      {
        if (expres.expression is expression.Literal lit) 
        {
          args = [.. args, lit.value];;
          continue;
        }
      }
      if (stmt is statement.Extern exter) 
      {
        if (luaEnv.parsers.TryGetValue(exter.name.lexeme, out LuaParser lua)) 
        {
          object[] pArgs = processArgs(luaEnv, exter.parameters, processor);
          LuaParserCall call = new(exter.name.lexeme, lua, pArgs);
          args = [.. args, call];
          continue;
        }
      }
      if (stmt is statement.Block block) 
      {
        processor.BlockCount += 1;
        string lbl = $"Block{processor.BlockCount}Label";
        processor.currentFunctionPos = new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y - processor.BlockCount * 2000 };
        Widget labelWidget = new("pneumaticcraft:label", new pieces.Coordinate { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y });
        Widget labelTextWidget = new("pneumaticcraft:text", new pieces.Coordinate { x = processor.currentFunctionPos.x + pieces.Label.Text.x, y = processor.currentFunctionPos.y });
        labelTextWidget.ExtraProperties["string"] = lbl;
        VisitorReturn initialReturn = new(new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y }, [labelWidget, labelTextWidget]);
        VisitorReturn[] rets = [initialReturn];
        processor.currentFunctionPos = new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y + 11 };
        foreach (statement state in block.statements) 
        {
          VisitorReturn? ret = processor.processStatement(state, true);
          if (ret == null) continue;
          processor.currentFunctionPos.y = ret.setLast.y;
          rets = [.. rets, ret];
        }
        Widget[][] widgetsMapped = [..rets.Select((visitorReturn) => visitorReturn.widgets)];
        Widget[] returnedWidgets = [];
        foreach (Widget[] mapped in widgetsMapped) 
        {
          returnedWidgets = [..returnedWidgets, ..mapped];
        }
        processor.additionalWidgets = [.. processor.additionalWidgets, ..returnedWidgets];
        processor.currentFunctionPos = new() { x = processor.currentFunctionPos.x, y = processor.currentFunctionPos.y + processor.BlockCount * 2000 };
        args = [.. args, lbl];
        continue;
      }
    }
    return args;
  }
  static readonly string[] exclusions = ["x", "newX", "y", "newY", "name", "width", "height"]; 
  public static VisitorReturn? process(LuaEnvironment luaEnv, pieces.Coordinate lastPos, ref pieces.Coordinate lastSize, statement.Extern @extern, AstProcessor processor) 
  {
    //LuaParser? parser = luaEnv.parsers[@extern.name.lexeme];
    if (luaEnv.parsers.TryGetValue(@extern.name.lexeme, out LuaParser parser)) 
    {
      if (!parser.validOutsideArguments)
      {
        processor.processingError.Invoke($"Cannot use {@extern.name.lexeme} outside of arguments!", @extern);
        return null;
      }
      Widget[] widgets = [];
      object[] args = processArgs(luaEnv, @extern.parameters, processor);
      LuaProcessorReturn[] ret = parser.process(lastPos.x, lastPos.y, args);
      if (ret.Length == 0) return null;
      foreach (LuaProcessorReturn proc in ret) 
      {
        Widget widget = new(proc.name, new() { x = proc.x, y = proc.y });
        foreach (TablePair dyn in proc.baseTable.Pairs) 
        {
          if (dyn.Key.Type != DataType.String) continue;
          string key = dyn.Key.String;
          if (!exclusions.Includes(key)) 
          {
            widget.ExtraProperties[key] = dyn.Value;
          }
        }
        widgets = [.. widgets, widget];
      }
      VisitorReturn visRet = new(new() { x = ret[0].newX, y = ret[0].newY }, widgets);
      return visRet;
    }
    return null;
  }
}