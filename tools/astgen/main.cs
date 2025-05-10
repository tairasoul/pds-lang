using tairasoul.pdsl.tools.ast;

namespace tairasoul.pdsl.tools.impl;

class Program 
{
  static void Main() 
  {
    ASTGenerator generator = new();
    generator.defineAST($"/home/{Environment.GetEnvironmentVariable("USER")}/development/c#/pdsl-lang/ast/expressions.cs", "Expression", "expressions", [], [
      "Binary : basename left, Token oper, basename right",
      "Grouping : basename expression",
      "Literal : object value",
      "Logical : basename left, Token oper, basename right",
      "Unary : Token oper, basename right"
    ]);
    
    generator.defineAST($"/home/{Environment.GetEnvironmentVariable("USER")}/development/c#/pdsl-lang/ast/statements.cs", "Statement", "statements", ["expr = tairasoul.pdsl.ast.expressions.Expression"], [
      "Expression : expr expression, int startColumn, int endColumn, int line",
      "Block : basename[] statements",
      "FunctionStatement : Token identifier, Block block",
      "Subroutine : Token label, int startColumn, int endColumn, int line",
      "Extern : Token name, basename[] parameters, int startColumn, int endColumn, int line",
      "Comment : string text",
      "Condition : string condition, expr.Binary[] comparisons",
      "Array : basename[] expressions, int startColumn, int endColumn, int line",
      "If : expr condition, basename thenBranch, basename? elseBranch"
    ]);
  }
}