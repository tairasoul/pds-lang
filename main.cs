using tairasoul.pdsl.lexer;
using tairasoul.pdsl.compiler;
using Newtonsoft.Json;

Utils.initializeExterns();

string configPath = Path.Join(Directory.GetCurrentDirectory(), "pdsl.json");
string configData = File.ReadAllText(configPath);
Config cfg = JsonConvert.DeserializeObject<Config>(configData) ?? new Config("./src", "./dist");
PdslCompiler compiler = new(cfg);
compiler.compile();