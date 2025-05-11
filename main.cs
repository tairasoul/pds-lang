using tairasoul.pdsl.lexer;
using tairasoul.pdsl.compiler;
using Newtonsoft.Json;

Utils.initializeExterns();

string configPath = Path.Join(Directory.GetCurrentDirectory(), "pdsl.json");
Config cfg;
if (File.Exists(configPath)) {
  string configData = File.ReadAllText(configPath);
  cfg = JsonConvert.DeserializeObject<Config>(configData) ?? new Config("./src", "./dist");
}
else 
{
  cfg = new Config("./src", "./dist");
}
PdslCompiler compiler = new(cfg);
compiler.compile();