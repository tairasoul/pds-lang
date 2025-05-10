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
  public static bool externsInitialized = false;
  public static void initializeExterns() 
  {
    externs.Add("for_each_item", TokenType.FOR_ITEM);
    externs.Add("for_each_coordinate", TokenType.FOR_COORDINATE);
    externs.Add("start", TokenType.START);
    externs.Add("area", TokenType.AREA);
    externs.Add("text", TokenType.TEXT);
    externs.Add("item_filter", TokenType.ITEM_FILTER);
    externs.Add("liquid_filter", TokenType.LIQUID_FILTER);
    externs.Add("fluid_filter", TokenType.LIQUID_FILTER);
    externs.Add("attack_entity", TokenType.ENTITY_ATTACK);
    externs.Add("dig", TokenType.DIG);
    externs.Add("harvest", TokenType.HARVEST);
    externs.Add("place", TokenType.PLACE);
    externs.Add("right_click_entity", TokenType.ENTITY_RIGHT_CLICK);
    externs.Add("right_click_block", TokenType.BLOCK_RIGHT_CLICK);
    externs.Add("pickup_item", TokenType.PICKUP_ITEM);
    externs.Add("drop_item", TokenType.DROP_ITEM);
    externs.Add("void_item", TokenType.VOID_ITEM);
    externs.Add("void_fluid", TokenType.VOID_FLUID);
    externs.Add("void_liquid", TokenType.VOID_FLUID);
    externs.Add("assign_item", TokenType.ITEM_ASSIGN);
    externs.Add("coordinate", TokenType.COORDINATE);
    externs.Add("coordinate_operator", TokenType.COORDINATE_OPERATOR);
    externs.Add("export_inventory", TokenType.INVENTORY_EXPORT);
    externs.Add("import_inventory", TokenType.INVENTORY_IMPORT);
    externs.Add("export_liquid", TokenType.LIQUID_EXPORT);
    externs.Add("import_liquid", TokenType.LIQUID_IMPORT);
    externs.Add("export_fluid", TokenType.LIQUID_EXPORT);
    externs.Add("import_fluid", TokenType.LIQUID_IMPORT);
    externs.Add("export_entity", TokenType.ENTITY_EXPORT);
    externs.Add("import_entity", TokenType.ENTITY_IMPORT);
    externs.Add("import_rf", TokenType.RF_IMPORT);
    externs.Add("export_rf", TokenType.RF_EXPORT);
    externs.Add("goto", TokenType.GOTO);
    externs.Add("teleport", TokenType.TELEPORT);
    externs.Add("emit_redstone", TokenType.EMIT_REDSTONE);
    externs.Add("label", TokenType.LABEL);
    externs.Add("jump", TokenType.JUMP);
    externs.Add("subroutine", TokenType.JUMP_SUB);
    externs.Add("wait", TokenType.WAIT);
    externs.Add("crafting", TokenType.CRAFTING);
    externs.Add("standby", TokenType.STANDBY);
    externs.Add("suicide", TokenType.SUICIDE);
    externs.Add("logistics", TokenType.LOGISTICS);
    externs.Add("edit_sign", TokenType.EDIT_SIGN);
    externs.Add("coordinate_condition", TokenType.CONDITION_COORDINATE);
    externs.Add("redstone_condition", TokenType.CONDITION_REDSTONE);
    externs.Add("light_condition", TokenType.CONDITION_LIGHT);
    externs.Add("item_inventory_condition", TokenType.CONDITION_ITEM_INVENTORY);
    externs.Add("block_condition", TokenType.CONDITION_BLOCK);
    externs.Add("liquid_condition", TokenType.CONDITION_LIQUID_INVENTORY);
    externs.Add("fluid_condition", TokenType.CONDITION_LIQUID_INVENTORY);
    externs.Add("entity_condition", TokenType.CONDITION_ENTITY);
    externs.Add("pressure_condition", TokenType.CONDITION_PRESSURE);
    externs.Add("item_condition", TokenType.CONDITION_ITEM);
    externs.Add("rf_condition", TokenType.CONDITION_RF);
    externs.Add("drone_item_condition", TokenType.DRONE_CONDITION_ITEM);
    externs.Add("drone_liquid_condition", TokenType.DRONE_CONDITION_LIQUID);
    externs.Add("drone_fluid_condition", TokenType.DRONE_CONDITION_LIQUID);
    externs.Add("drone_pressure_condition", TokenType.DRONE_CONDITION_PRESSURE);
    externs.Add("drone_entity_condition", TokenType.DRONE_CONDITION_ENTITY);
    externs.Add("drone_upgrades_condition", TokenType.DRONE_CONDITION_UPGRADE);
    externs.Add("drone_rf_condition", TokenType.DRONE_CONDITION_RF);
    externs.Add("computer_control", TokenType.COMPUTER_CONTROL);
    externs.Add("external_program", TokenType.EXTERNAL_PROGRAM);
  }
  public static Dictionary<string, TokenType> externs = [];
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
    if (!exists) exists = Utils.externs.TryGetValue(text, out type);
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