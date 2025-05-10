using expressions = tairasoul.pdsl.ast.expressions.Expression;
using statements = tairasoul.pdsl.ast.statements.Statement;
using tairasoul.pdsl.lexer;
using tairasoul.pdsl.tokens;
using System.Reflection.Emit;
using Newtonsoft.Json;

namespace tairasoul.pdsl.parser;

class Parser 
{
  private Token[] tokens;
  private Token startOfMatch;
  private int current = 0;
  private string[] validLabels = [];
  public event Action<int, int, int, string> parsingError = (line, start, end, reason) => 
  {
    
  };
  
  private bool labelsIncludes(string lexeme) 
  {
    foreach (string label in validLabels) 
    {
      if (label == lexeme) return true;
    }
    return false;
  }
  
  public Parser(Token[] tokens) 
  {
    this.tokens = tokens;
  }
  
  public statements[] parse() 
  {
    statements[] stmts = [];
    while (!isAtEnd()) 
    {
      statements? stmt = declaration();
      if (stmt != null)
        stmts = [..stmts, stmt];
    }
    return stmts;
  }
  
  private statements? declaration() 
  {
    Token? current = peek() ?? throw new Exception("No token at current index.");
    if (labelsIncludes(current.lexeme)) 
    {
      startOfMatch = current;
      while (advance().type != TokenType.RIGHT_PAREN) {};
      return funcSubroutine(current);
    }
    if (Utils.externs.ContainsKey(current.lexeme))
      return external();
      
    if (match(TokenType.LEFT_SQ_BRACE)) 
    {
      Token leftSq = previous();
      startOfMatch = leftSq;
      statements[] statements = [];
      while (!check(TokenType.RIGHT_SQ_BRACE) && !isAtEnd()) 
      {
        statements? statement = declaration();
        if (statement == null) continue;
        statements = [..statements, statement];
      }
      
      Token? end = consume(TokenType.RIGHT_SQ_BRACE, $"Expected ']' to end '[' at line {leftSq.line}, column {leftSq.startColumn}-{leftSq.endColumn}.");
      if (end == null) return null;
      return new statements.Array(statements, startOfMatch.endColumn, end.endColumn, startOfMatch.line);
    }
    
    if (match(TokenType.FUNCTION)) return functionStatement();
    if (check(TokenType.COMMENT)) return comment();
    
    return statement();
  }
  
  private statements? external() 
  {
    Token? identifier = consumeMultiple("Expected identifier for pneumaticcraft function call.", [..Utils.externs.Values]);
    if (identifier == null) return null;
    consume(TokenType.LEFT_PAREN, "Expected '(' after function name.");
    statements[] args = [];
    while (!check(TokenType.RIGHT_PAREN) && !isAtEnd()) {
      statements? result = declaration();
      if (result != null)
        args = [..args, result];
    }
    Token? end = consume(TokenType.RIGHT_PAREN, "Expected ')' to close function call.");
    if (end == null) return null;
    return new statements.Extern(identifier, args, identifier.endColumn, end.endColumn, identifier.line);
  }
  
  private statements? comment() 
  {
    Token? comment = advance();
    if (comment == null) return null;
    return new statements.Comment(comment.lexeme.Replace("#", "").Trim());
  }
  
  private statements funcSubroutine(Token label) 
  {
    return new statements.Subroutine(label, label.endColumn, label.endColumn, label.line);
  }
  
  private statements? functionStatement() 
  {
    Token? name = consume(TokenType.IDENTIFIER, $"Expected function name.");
    if (name == null) return null;
    if (!Utils.externs.ContainsKey(name.lexeme))
      validLabels = [..validLabels, name.lexeme];
    statements[]? codeBlock = block();
    if (codeBlock == null) return null;
    return new statements.FunctionStatement(name, new statements.Block(codeBlock));
  }
  
  private statements? statement()
  {
    if (match(TokenType.LEFT_BRACE)) return new statements.Block(block());
    return expressionStatement();
  }
  
  private statements[]? block() 
  {
    statements[] stmts = [];
    
    if (check(TokenType.LEFT_BRACE)) advance();
    
    while (!check(TokenType.RIGHT_BRACE) && !isAtEnd()) 
    {
      statements? dec = declaration();
      if (dec != null)
        stmts = [..stmts, dec];
    }
    
    consume(TokenType.RIGHT_BRACE, "Expected '}' to close code block.");
    
    return stmts;
  }
  
  private statements? expressionStatement() 
  {
    Token start = previous();
    expressions? expr = expression();
    if (expr == null) return null;
    Token end = previous();
    return new statements.Expression(expr, start.endColumn, end.endColumn, start.line);
  }
  
  private expressions? expression() 
  {
    return or();
  }
  
  private expressions? or() 
  {
    expressions? expr = and();
    if (expr == null) return null;
    while (match(TokenType.OR)) 
    {
      Token oper = previous();
      expressions? right = and();
      if (right == null) return null;
      expr = new expressions.Logical(expr, oper, right);
    }
    return expr;
  }
  
  private expressions? and() 
  {
    expressions? expr = equality();
    if (expr == null) return null;
    while (match(TokenType.AND)) 
    {
      Token oper = previous();
      expressions? right = equality();
      if (right == null) return null;
      expr = new expressions.Logical(expr, oper, right);
    }
    return expr;
  }
  
  private expressions? equality() 
  {
    expressions? expr = primary();
    if (expr == null) return null;
    
    while (match(TokenType.EQUAL_EQUAL)) 
    {
      Token oper = previous();
      expressions? right = comparison();
      if (right == null) return null;
      expr = new expressions.Binary(expr, oper, right);
    }
    return expr;
  }
  
  private expressions? comparison() 
  {
    expressions? expr = primary();
    if (expr == null) return null;
    
    while (match(TokenType.GREATER_EQUAL, TokenType.LESS_EQUAL)) {
      Token oper = previous();
      expressions? right = primary();
      if (right == null) return null;
      expr = new expressions.Binary(expr, oper, right);
    }
    
    return expr;
  }
  
  private expressions? primary() 
  {
    if (match(TokenType.NUMBER, TokenType.STRING)) {
      Token prev = previous();
      return new expressions.Literal(prev.literal);
    }
    
    if (match(TokenType.TRUE)) {
      return new expressions.Literal(true);
    }
    if (match(TokenType.FALSE)) {
      return new expressions.Literal(false);
    }
    
    Token current = peek();
    parsingError.Invoke(current.line, current.startColumn, current.endColumn, $"Unexpected token of type {current.type} found.");
    advance();
    return null;
  }
  
  private Token? consumeMultiple(string? error, TokenType[] tokens) 
  {
    foreach(TokenType token in tokens) 
    {
      Token? consumed = consume(token, null);
      if (consumed != null)
        return consumed;
    }
    Token prev = previous();
    if (error != null)
      parsingError.Invoke(prev.line, prev.startColumn, prev.endColumn, error);
    return null;
  }
  
  private Token? consume(TokenType token, string? error) 
  {
    if (check(token)) return advance();
    
    Token nextToken = peek();
    if (error != null)
      parsingError.Invoke(nextToken.line, nextToken.startColumn, nextToken.endColumn, error);
    return null;
  }
  
  private bool match(params TokenType[] types) 
  {
    foreach (TokenType token in types) {
      if (check(token)) 
      {
        advance();
        return true;
      }
    }
    return false;
  }
  
  private bool check(TokenType type) 
  {
    if (isAtEnd()) return false;
    return peek().type == type;
  }
  
  private Token advance() 
  {
    if (!isAtEnd()) current++;
    return previous();
  }
  
  private bool isAtEnd() 
  {
    return peek().type == TokenType.EOF;
  }
  
  private Token peek() 
  {
    return tokens[current];
  }
  
  private Token previous() 
  {
    return tokens[current - 1];
  }
}