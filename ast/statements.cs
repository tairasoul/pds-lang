using tairasoul.pdsl.lexer;
using expr = tairasoul.pdsl.ast.expressions.Expression;

namespace tairasoul.pdsl.ast.statements;

abstract class Statement {
  public class Expression : Statement {
    public expr expression;
    public int startColumn;
    public int endColumn;
    public int line;
    public Expression(expr expression, int startColumn, int endColumn, int line) {
      this.expression = expression;
      this.startColumn = startColumn;
      this.endColumn = endColumn;
      this.line = line;
    }
  }
  public class Block : Statement {
    public Statement[] statements;
    public Block(Statement[] statements) {
      this.statements = statements;
    }
  }
  public class FunctionStatement : Statement {
    public Token identifier;
    public Block block;
    public FunctionStatement(Token identifier, Block block) {
      this.identifier = identifier;
      this.block = block;
    }
  }
  public class Subroutine : Statement {
    public Token label;
    public int startColumn;
    public int endColumn;
    public int line;
    public Subroutine(Token label, int startColumn, int endColumn, int line) {
      this.label = label;
      this.startColumn = startColumn;
      this.endColumn = endColumn;
      this.line = line;
    }
  }
  public class Extern : Statement {
    public Token name;
    public Statement[] parameters;
    public int startColumn;
    public int endColumn;
    public int line;
    public Extern(Token name, Statement[] parameters, int startColumn, int endColumn, int line) {
      this.name = name;
      this.parameters = parameters;
      this.startColumn = startColumn;
      this.endColumn = endColumn;
      this.line = line;
    }
  }
  public class Comment : Statement {
    public string text;
    public Comment(string text) {
      this.text = text;
    }
  }
  public class Condition : Statement {
    public string condition;
    public expr.Binary[] comparisons;
    public Condition(string condition, expr.Binary[] comparisons) {
      this.condition = condition;
      this.comparisons = comparisons;
    }
  }
  public class Array : Statement {
    public Statement[] expressions;
    public int startColumn;
    public int endColumn;
    public int line;
    public Array(Statement[] expressions, int startColumn, int endColumn, int line) {
      this.expressions = expressions;
      this.startColumn = startColumn;
      this.endColumn = endColumn;
      this.line = line;
    }
  }
  public class If : Statement {
    public expr condition;
    public Statement thenBranch;
    public Statement? elseBranch;
    public If(expr condition, Statement thenBranch, Statement? elseBranch) {
      this.condition = condition;
      this.thenBranch = thenBranch;
      this.elseBranch = elseBranch;
    }
  }
}