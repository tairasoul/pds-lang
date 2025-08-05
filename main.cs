using tairasoul.pdsl.compiler;
using Newtonsoft.Json;

string configPath = Path.Join(Directory.GetCurrentDirectory(), "pdsl.json");
Config cfg;
if (File.Exists(configPath)) {
  string configData = File.ReadAllText(configPath);
  cfg = JsonConvert.DeserializeObject<Config>(configData) ?? new Config("./src", "./dist", true);
}
else 
{
  throw new Exception("No config in current directory!");
}
PdslCompiler compiler = new(cfg);
compiler.compile();