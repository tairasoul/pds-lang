using System.Text.RegularExpressions;
using tairasoul.pdsl.tokens;

namespace tairasoul.pdsl.lexer;

class Token(TokenType type, string lexeme, object? literal, int startColumn, int endColumn, int line)
{
  public TokenType type = type;
  public string lexeme = lexeme;
  public object? literal = literal;
  public int startColumn = startColumn;
  public int endColumn = endColumn;
  public int line = line;
  
  public override string ToString() 
  {
      return $"{type} {lexeme} {literal}";
  }
}
static class Utils 
{
  public static bool isNumeric(char character) 
  {
    Regex regex = new("[0-9]");
    return regex.IsMatch(character.ToString());
  }
  
  public static bool isAlpha(char character) 
  {
    Regex regex = new("[a-z]|[A-Z]|_");
    return regex.IsMatch(character.ToString());
  }
  
  public static bool isAlphanumeric(char character) 
  {
      return isNumeric(character) || isAlpha(character);
  }
}

class Lexer
{
  private string source;
  private Token[] tokens = [];
  private int start = 0;
  private int startCol = 1;
  private int currentCol = 1;
  private int current = 0;
  private int line = 1;
  private Dictionary<string, TokenType> keywords = [];
  public event Action<int, int, int, string> lexicalError = (line, from, to, reason) => 
  {
      
  };
  
  public Lexer(string source) 
  {
    this.source = source;
    keywords.Add("function", TokenType.FUNCTION);
    keywords.Add("true", TokenType.TRUE);
    keywords.Add("false", TokenType.FALSE);
    //keywords.Add("else", TokenType.ELSE);
    //keywords.Add("&", TokenType.AND);
    //keywords.Add("and", TokenType.AND);
    //keywords.Add("|", TokenType.OR);
    //keywords.Add("or", TokenType.OR);
  }
  
  private bool isAtEnd() 
  {
    return current >= source.Length;
  }
  
  private char advance() 
  {
    currentCol++;
    return source.ElementAt(current++);
  }
  
  public Token[] LexSource() 
  {
    while (!isAtEnd()) 
    {
      startCol = currentCol;
      start = current;
      scanToken();
    }
    
    tokens = [..tokens, new(TokenType.EOF, "", null, tokens.Last().endColumn, tokens.Last().endColumn, line)];
    return tokens;
  }
  
  private void scanToken() 
  {
    char c = advance();
    switch (c) 
    {
      case '(': addToken(TokenType.LEFT_PAREN); break;
      case ')': addToken(TokenType.RIGHT_PAREN); break;
      case '{': addToken(TokenType.LEFT_BRACE); break;
      case '}': addToken(TokenType.RIGHT_BRACE); break;
      case '[': addToken(TokenType.LEFT_SQ_BRACE); break;
      case ']': addToken(TokenType.RIGHT_SQ_BRACE); break;
      case ',': addToken(TokenType.COMMA); break;
      case '"': string_(); break;
      case ';': addToken(TokenType.SEMICOLON); break;
      case ' ':
      case '\r':
      case '\t':
        break;
      case '\n':
        startCol = 1;
        currentCol = 1;
        line++;
        break;
      case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
      case '<': if (!match('=')) { lexicalError.Invoke(line, startCol, currentCol, "< operator is not supported in PDSL!"); break; } addToken(TokenType.LESS_EQUAL); break;
      case '>': if (!match('=')) { lexicalError.Invoke(line, startCol, currentCol, "> operator is not supported in PDSL!"); break; } addToken(TokenType.GREATER_EQUAL); break;
      case '&': addToken(TokenType.AND); break;
      case '|': addToken(TokenType.OR); break; 
      case '#':
        while (peek() != '\n' && !isAtEnd()) advance();
        addToken(TokenType.COMMENT);
        break;
      default:
        if (Utils.isNumeric(c) || (c == '-' && Utils.isNumeric(peek()))) 
        {
          number();
        }
        else if (Utils.isAlpha(c) || keywords.ContainsKey(c.ToString())) 
        {
          identifier();
        }
        else 
        {
          lexicalError.Invoke(line, startCol, currentCol, $"Unexpected character {c}");
        }
        break;
    }
  }
  
  private void identifier() 
  {
    while (Utils.isAlphanumeric(peek())) advance();
    
    string text = source[start..current];
    bool exists = keywords.TryGetValue(text, out TokenType type);
    if (!exists) type = TokenType.IDENTIFIER;
    addToken(type);
  }
  
  private void number() 
  {
    while (Utils.isNumeric(peek())) advance();
    
    bool isFloat = false;
    
    if (peek() == '.' && Utils.isNumeric(peekNext())) 
    {
      isFloat = true;
      advance();
      while (Utils.isNumeric(peek())) advance();
    }
    
    string text = source[start..current];
    addToken(TokenType.NUMBER, isFloat ? float.Parse(text) : int.Parse(text));
  }

  private void string_() 
  {
    while (peek() != '"' && !isAtEnd()) 
    {
      if (peek() == '\n') line++;
      advance();
    }
    
    if (isAtEnd()) 
    {
      lexicalError.Invoke(line, startCol, currentCol, "Unterminated string.");
      return;
    }
    
    advance();
    
    string value = source[(start + 1)..(current - 1)];
    addToken(TokenType.STRING, value);
  }
  
  private char peekNext() 
  {
    if (current + 1 >= source.Length) return '\0';
    return source.ElementAt(current + 1);
  }
  
  private char peek() 
  {
    if (isAtEnd()) return '\0';
    return source.ElementAt(current);
  }
  
  private bool match(char expected) 
  {
    if (isAtEnd()) return false;
    if (source.ElementAt(current) != expected) return false;
    
    current++;
    return true;
  }
  
  private void addToken(TokenType token) 
  {
    addToken(token, null);
  }
  
  private void addToken(TokenType token, object? literal) 
  {
    string text = source[start..current];
    tokens = [..tokens, new(token, text, literal, startCol, currentCol, line)];
  }
}