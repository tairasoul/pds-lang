using System.Reflection;
using MoonSharp.Interpreter;
using tairasoul.pdsl.extensions;

namespace tairasoul.pdsl.luapiece;

class LuaArgument 
{
  public string name;
  public string[] types;
  public bool required;
  public Closure? validate;
}

[MoonSharpUserData]
class LuaParserCall(string identifier, LuaParser parser, object[] args) 
{
  public string identifier = identifier;
  public object[] objects = args;
  public LuaParser parser = parser;
}

[MoonSharpUserData]
class LuaParser(string identifier, Closure processor, Script ourScript, LuaArgument[]? arguments, bool validOutsideArguments = true)
{
  public readonly string identifier = identifier;
  internal readonly LuaArgument[]? arguments = arguments;
  public readonly Closure processor = processor;
  public readonly bool validOutsideArguments = validOutsideArguments;
  internal Script ourScript = ourScript;
  public LuaProcessorReturn[] process(int x, int y, params object[] args) 
  {
    bool valid = ValidateArguments(args);
    if (!valid) return [];
    DynValue dyn = ourScript.Call(processor, [x, y, .. args]);
    if (dyn.IsNil()) return [];
    Table tbl = dyn.Table;
    LuaProcessorReturn[] returns = [];
    foreach (DynValue val in tbl.Values)
    {
      Table t = val.Table;
      LuaProcessorReturn ret = new()
      {
        baseTable = t,
        height = (int)t.Get("height").Number,
        width = (int)t.Get("width").Number,
        newX = t.Get("newX").Type == DataType.Number ? (int)t.Get("newX").Number : 0,
        newY = t.Get("newY").Type == DataType.Number ? (int)t.Get("newY").Number : 0,
        x = (int)t.Get("x").Number,
        y = (int)t.Get("y").Number,
        name = t.Get("name").String
      };
      returns = [..returns, ret];
    }
    return returns;
  }
  public bool ValidateArguments(params object[] args) 
  {
    if (arguments == null) return true;
    if (arguments.Length == 0) return true;
    for (int i = 0; i < args.Length; i++) 
    {
      LuaArgument arg = arguments[i];
      if (arg.required && args.ElementAtOrDefault(i) == null) 
      {
        LuaEnvironment.LuaError.Invoke($"Expected argument for required argument {arg.name}", i);
        return false;
      }
      if (!arg.required && args.ElementAtOrDefault(i) == null) break;
      object argAt = args[i];
      string type = argAt.GetType().Name.ToLower();
      bool bypass = false;
      if (argAt is LuaParserCall call) 
      {
        type = call.identifier.ToLower();
      }
      if (argAt.GetType().IsArray) 
      {
        dynamic dyn = argAt;
        if (dyn.Length == 0) bypass = true;
        if (dyn.Length > 0 && dyn[0].GetType().GetField("identifier") != null)
          type = dyn[0].identifier;
        else if (argAt.GetType() == typeof(object[])) 
        {
          HashSet<Type> found = [];
          foreach (object arrItem in argAt as object[]) 
          {
            found.Add(arrItem.GetType());
          }
          if (found.Count == 1) 
          {
            type = found.First().Name.ToLower();
          }
        }
        if (!type.EndsWith("[]"))
          type += "[]";
      }
      bool valid;
      if (arg.validate != null) 
      {
        DynValue dv = ourScript.Call(arg.validate, [argAt]);
        valid = dv.Boolean;
      }
      else 
      {
        if (!bypass)
          valid = arg.types.Includes(type);
        else
          valid = true;
      }
      if (!valid) 
      {
        LuaEnvironment.LuaError.Invoke($"Expected argument of type {arg.types.ConcatString((inp, first) => first ? inp : $" | {inp}")} for {arg.name}, got {type}", i);
        return false;
      }
    }
    return true;
  }
}

[MoonSharpUserData]
class LuaProcessorReturn
{
  public string name;
  public int x;
  public int y;
  public int newX;
  public int newY;
  public int width;
  public int height;
  public Table baseTable;
}

class LuaEnvironment 
{
  internal static Script mainEnv = new();
  internal readonly Dictionary<string, LuaParser> parsers = [];
  internal static Action<string, int> LuaWarning = (a, b) => { };
  internal static Action<string, int> LuaError = (a, b) => { };
  internal static Action<string> LuaPullWarning = (a) => { };
  internal static Action<string> LuaPullError = (a) => { };
  internal static Action<string> LuaFuncWarning = (a) => { };
  internal static Action<string> LuaFuncError = (a) => { };
  public LuaEnvironment() 
  {
    UserData.RegisterAssembly(Assembly.GetEntryAssembly());
    SetupScript(mainEnv);
  }
  
  private LuaParser? GetParser(string name)
  {
    if (parsers.TryGetValue(name, out LuaParser parser))
      return parser;
    return null;
  }
  private void SetupScript(Script script)
  {
    script.Globals["getParser"] = (Func<string, LuaParser?>)GetParser;
    script.Globals["warn"] = (string warning) => LuaFuncError.Invoke(warning);
    script.Globals["error"] = (string error) => LuaFuncError.Invoke(error);
    script.Globals["gExists"] = (string prop) => script.Globals[prop] != null;
  }
  
  internal void PullParser(string code, string fileName, bool allowOverwrites = true, bool isInternal = false)
  {
    //Script isolated = new();
    //SetupScript(isolated);
    DynValue dyn = mainEnv.DoString(code);
    if (dyn.IsVoid() || dyn.IsNil()) return;
    string identifier = dyn.Table.Get("identifier").String;
    Closure pars = dyn.Table.Get("processor").Function;
    Table? ArgsTable = dyn.Table.Get("arguments").Table;
    DynValue validDyn = dyn.Table.Get("validOutsideArguments");
    bool valid = true;
    if (validDyn.IsNotNil() && validDyn.Type == DataType.Boolean) 
    {
      valid = validDyn.Boolean;
    }
    LuaArgument[]? args = null;
    if (ArgsTable != null) 
    {
      args = [];
      foreach (DynValue argVal in ArgsTable.Values) 
      {
        if (argVal.Type != DataType.Table) continue;
        Table t = argVal.Table;
        string name = t.Get("name").String;
        Table typeT = t.Get("types").Table;
        //string type = t.Get("type").String;
        DynValue reqDyn = t.Get("required");
        bool required = true;
        if (reqDyn.IsNotNil() && reqDyn.Type == DataType.Boolean)
        {
          required = reqDyn.Boolean;
        }
        Closure closure = t.Get("validate").Function;
        string[] types = [];
        foreach (DynValue val in typeT.Values) 
        {
          if (val.Type == DataType.String) 
          {
            types = [.. types, val.String];
          }
        }
        LuaArgument arg = new()
        {
          name = name,
          types = types,
          required = required,
          validate = closure
        };
        args = [..args, arg];
      }
    }
    LuaParser parser = new(identifier, pars, mainEnv, args, valid);
    if (parsers.ContainsKey(identifier)) 
    {
      if (isInternal) 
      {
        if (allowOverwrites)
          LuaPullWarning.Invoke($"{fileName} is overwriting an internal piece identifier! This can be ignored if this is intentional.");
        else 
        {
          LuaPullError.Invoke($"{fileName} is overwriting an internal piece identifier! Enable overwrites in config if this is intentional.");
          return;
        }
      }
      else 
      {
        if (allowOverwrites)
          LuaPullWarning.Invoke($"{fileName} is overwriting a piece identifier! This can be ignored if this is intentional.");
        else 
        {
          LuaPullError.Invoke($"{fileName} is overwriting a piece identifier! Enable overwrites in config if this is intentional.");
          return;
        }
      }
    }
    parsers[identifier] = parser;
  }
}