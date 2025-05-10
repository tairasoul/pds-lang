using tairasoul.pdsl.parser;
using tairasoul.pdsl.processor;
using tairasoul.pdsl.ast.statements;
using tairasoul.pdsl.visitors.returnTypes;
using tairasoul.pdsl.postprocessed;
using tairasoul.pdsl.lexer;

namespace tairasoul.pdsl.compiler;

class Config(string sourceDir, string outDir)
{
  public string sourceDir = sourceDir;
  public string outDir = outDir;
}

class PdslCompiler(Config cfg)
{
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

  public void compile() 
  {
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
        Console.WriteLine($"[ERROR: pdsl/Lexer] [{line}:{from}-{to}] {reason}");
      };
      Token[] tokens = lexer.LexSource();
      Parser parser = new(tokens);
      parser.parsingError += (line, from, to, reason) => {
        Console.WriteLine($"[ERROR: pdsl/Parser] [{line}:{from}-{to}] {reason}");
      };
      Statement[] result = parser.parse();
      AstProcessor processor = new(result);
      processor.processingError += (reason, statement) => {
        Console.WriteLine($"[ERROR: pdsl/Processor] [{statement.line}:{statement.startColumn}-{statement.endColumn}] {reason}");
      };
      Widget[] widgets = processor.getResult();
      PneumaticraftJsonObject pneumaticraftJson = new(3, widgets);
      string outputDir = Path.Join(compilerConfig.outDir, $"{basename}.json");
      File.WriteAllText(outputDir, pneumaticraftJson.GetJson());
    }
    DateTime end = DateTime.Now;
    TimeSpan timeSpent = end - start;
    Console.WriteLine($"Took {FormatTimeSpan(timeSpent)} to compile {files.Length} files.");
  }
}