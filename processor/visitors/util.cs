using expression = tairasoul.pdsl.ast.expressions.Expression;
using statement = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.visitors.errors;

namespace tairasoul.pdsl.visitors.util;

static class VisitorUtil 
{
  internal static (object[], (string paramName, Type type, object? defaultValue)[]) PrepareParamSpec(
      string externName, (Type type, object? defaultValue)[] specs)
  {
      var info = new (string paramName, Type type, object? defaultValue)[specs.Length];
      for (int i = 0; i < specs.Length; i++)
      {
          info[i] = ($"param{i}", specs[i].type, specs[i].defaultValue);
      }
      return (new object[specs.Length], info);
  }

  internal static object[] GetLiteralParams(statement[] expressions, string externName, params (Type type, object? defaultValue)[] specs)
  {
      var (results, paramInfo) = PrepareParamSpec(externName, specs);

      for (int i = 0; i < paramInfo.Length; i++)
      {
      var (_, expectedType, defaultValue) = paramInfo[i];

          if (i < expressions.Length)
          {
              var expr = expressions[i];
              if (expr is not statement.Expression exprStmt)
              {
                  throw new VisitorError(
                      $"Expected expression for parameter {i + 1} of {externName}, got {expr.GetType().Name}", expr
                  );
              }

              var inner = exprStmt.expression;
              if (inner is not expression.Literal lit)
              {
                  throw new VisitorError(
                      $"Expected literal expression for parameter {i} of {externName}, got {inner.GetType().Name}", exprStmt
                  );
              }

              if (lit.value == null || !expectedType.IsAssignableFrom(lit.value.GetType()))
              {
                  throw new VisitorError(
                      $"Expected literal of type {expectedType.Name} for parameter {i} of {externName}, got {lit.value?.GetType().Name ?? "null"}", exprStmt
                  );
              }

              results[i] = lit.value;
          }
          else
          {
            if (defaultValue != null)
              results[i] = defaultValue;
          }
      }

      return results;
  }
}