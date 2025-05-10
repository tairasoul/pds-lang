using tairasoul.pdsl.ast.statements;

namespace tairasoul.pdsl.visitors.errors;

class VisitorError(string reason, Statement statement) : Exception
{
  public string reason = reason;
  public Statement statement = statement;
}