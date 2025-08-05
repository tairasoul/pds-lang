using tairasoul.pdsl.parser;
using tairasoul.pdsl.processor;
using tairasoul.pdsl.ast.statements;
using tairasoul.pdsl.visitors.returnTypes;
using tairasoul.pdsl.postprocessed;
using tairasoul.pdsl.lexer;
using tairasoul.pdsl.luapiece;
using System.Reflection;
using System.Text;

namespace tairasoul.pdsl.compiler;

class Config(string sourceDir, string outDir, bool allowOverwrites)
{
  public readonly string sourceDir = sourceDir;
  public readonly bool allowOverwrites = allowOverwrites;
  public readonly string outDir = outDir;
}

class PdslCompiler(Config cfg)
{
  readonly LuaEnvironment env = new();
  readonly Config compilerConfig = cfg;

  static string FormatTimeSpan(TimeSpan timeSpan)
  {
      var parts = new List<string>();

      if (timeSpan.Hours > 0)
          parts.Add($"{timeSpan.Hours} hour{(timeSpan.Hours != 1 ? "s" : "")}");
      if (timeSpan.Minutes > 0)
          parts.Add($"{timeSpan.Minutes} minute{(timeSpan.Minutes != 1 ? "s" : "")}");
      if (timeSpan.Seconds > 0)
          parts.Add($"{timeSpan.Seconds} second{(timeSpan.Seconds != 1 ? "s" : "")}");
      if (timeSpan.Milliseconds > 0)
          parts.Add($"{timeSpan.Milliseconds} millisecond{(timeSpan.Milliseconds != 1 ? "s" : "")}");
      if (parts.Count == 0)
          return "0 milliseconds";
      return string.Join(" ", parts);
  }
  
  private static byte[] ExtractResource(string filename)
  {
    Assembly a = Assembly.GetExecutingAssembly();
    using Stream resFilestream = a.GetManifestResourceStream(filename);
    if (resFilestream == null) return null;
    byte[] ba = new byte[resFilestream.Length];
    resFilestream.Read(ba, 0, ba.Length);
    return ba;
  }
  
  private void SetupLua() 
  {
    GrabBuiltinLuaFiles();
    ReadPieceLua();
    LuaEnvironment.LuaError += (reason, arg) =>
    {
      Console.WriteLine($"[pdsl/LuaEnv: ERROR] {reason}, arg {arg}");
    };
    LuaEnvironment.LuaFuncError += (reason) =>
    {
      Console.WriteLine($"[pdsl/LuaEnv(FUNC): ERROR] {reason}");
    };
    LuaEnvironment.LuaFuncWarning += (reason) =>
    {
      Console.WriteLine($"[pdsl/LuaEnv(FUNC): WARNING] {reason}");
    };
  }
  
  private void GrabBuiltinLuaFiles() 
  {
    string[] streamNames = Assembly.GetCallingAssembly().GetManifestResourceNames();
    foreach (string streamN in streamNames)
    {
      if (streamN.EndsWith(".lua")) 
      {
        byte[] buffer = ExtractResource(streamN);
        string code = Encoding.UTF8.GetString(buffer);
        Console.WriteLine($"Pulling builtin parser from {Path.GetFileName(streamN)}");
        env.PullParser(code, Path.GetFileName(streamN), compilerConfig.allowOverwrites, true);
      }
    }
  }
  
  private void ReadPieceDir(string dir) 
  {
    string[] files = Directory.GetFiles(dir);
    foreach (string file in files)
    {
      string code = File.ReadAllText(file);
      Console.WriteLine($"Pulling parser from {Path.GetFileName(file)}");
      env.PullParser(code, Path.GetFileName(file), compilerConfig.allowOverwrites, false);
    }
    string[] dirs = Directory.GetDirectories(dir);
    foreach (string dire in dirs) 
    {
      ReadPieceDir(dire);
    }
  }

  private void ReadPieceLua()
  {
    string PieceLuaDir = Path.Join(Directory.GetCurrentDirectory(), "lua");
    if (Directory.Exists(PieceLuaDir)) 
    {
      ReadPieceDir(PieceLuaDir);
    }
  }

  public void compile() 
  {
    SetupLua();
    DateTime start = DateTime.Now;
    if (!Directory.Exists(compilerConfig.outDir)) 
    {
      Directory.CreateDirectory(compilerConfig.outDir);
    }
    string[] files = [..Directory.EnumerateFiles(compilerConfig.sourceDir).Where((str) => str.EndsWith(".pdsl"))];
    foreach (string inputFile in files) 
    {
      string basename = Path.GetFileNameWithoutExtension(inputFile);
      Lexer lexer = new(File.ReadAllText(inputFile));
      lexer.lexicalError += (line, from, to, reason) => 
      {
        Console.WriteLine($"[pdsl/Lexer: ERROR] [{line}:{from}-{to}] {reason}");
      };
      Token[] tokens = lexer.LexSource();
      Parser parser = new(tokens, env);
      parser.parsingError += (line, from, to, reason) => {
        Console.WriteLine($"[pdsl/Parser: ERROR] [{line}:{from}-{to}] {reason}");
      };
      Statement[] result = parser.parse();
      AstProcessor processor = new(result, env);
      processor.processingError += (reason, statement) => {
        if (statement is Statement.Expression expr)
          Console.WriteLine($"[pdsl/Processor: ERROR] [{expr.line}:{expr.startColumn}-{expr.endColumn}] {reason}");
      };
      Widget[] widgets = processor.getResult();
      PneumaticraftJsonObject pneumaticraftJson = new(3, widgets);
      string outputDir = Path.Join(compilerConfig.outDir, $"{basename}.json");
      File.WriteAllText(outputDir, pneumaticraftJson.GetJson());
    }
    DateTime end = DateTime.Now;
    TimeSpan timeSpent = end - start;
    Console.WriteLine($"Took {FormatTimeSpan(timeSpent)} to compile {files.Length} file{(files.Length > 1 ? "s" : "")}.");
  }
}