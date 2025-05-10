using tairasoul.pdsl.lexer;

namespace tairasoul.pdsl.ast.expressions;

abstract class Expression {
  public class Binary : Expression {
    public Expression left;
    public Token oper;
    public Expression right;
    public Binary(Expression left, Token oper, Expression right) {
      this.left = left;
      this.oper = oper;
      this.right = right;
    }
  }
  public class Grouping : Expression {
    public Expression expression;
    public Grouping(Expression expression) {
      this.expression = expression;
    }
  }
  public class Literal : Expression {
    public object value;
    public Literal(object value) {
      this.value = value;
    }
  }
  public class Logical : Expression {
    public Expression left;
    public Token oper;
    public Expression right;
    public Logical(Expression left, Token oper, Expression right) {
      this.left = left;
      this.oper = oper;
      this.right = right;
    }
  }
  public class Unary : Expression {
    public Token oper;
    public Expression right;
    public Unary(Token oper, Expression right) {
      this.oper = oper;
      this.right = right;
    }
  }
}