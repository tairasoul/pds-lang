using System.Text;

namespace tairasoul.pdsl.tools.ast;

class ASTGenerator 
{
  private void writeToFile(FileStream file, string data) 
  {
    file.Write(Encoding.Default.GetBytes(data));
  }
  public void defineAST(string outputFile, string basename, string namespaceName, string[] extraUsings, string[] types) 
  {
    if (File.Exists(outputFile)) 
    {
      File.Delete(outputFile);
    }
    FileStream output = File.Open(outputFile, FileMode.Create);
    writeToFile(output, $"using tairasoul.pdsl.lexer;\n");
    foreach (string extraUsing in extraUsings) 
    {
      writeToFile(output, $"using {extraUsing};\n");
    }
    writeToFile(output, $"\nnamespace tairasoul.pdsl.ast.{namespaceName.ToLower()};\n\n");
    writeToFile(output, $"abstract class {basename} {{\n");
    foreach (string arr in types) {
      string type = arr;
      string baseName = type.Split(":")[0].Trim();
      string[] fields = type.Split(":")[1].Trim().Replace("basename", basename).Split(", ");
      writeToFile(output, $"  public class {baseName} : {basename} {{\n");
      foreach (string field in fields) 
      {
        string name = field.Split(" ")[1];
        string fieldType = field.Split(" ")[0];
        writeToFile(output, $"    public {fieldType} {name};\n");
      }
      writeToFile(output, $"    public {baseName}(");
      string constructorParams = "";
      foreach (string field in fields) 
      {
        string name = field.Split(" ")[1];
        string fieldType = field.Split(" ")[0];
        if (constructorParams != "")
          constructorParams = $"{constructorParams}, {fieldType} {name}";
        else
          constructorParams = $"{fieldType} {name}";
      }
      writeToFile(output, constructorParams);
      writeToFile(output, $") {{\n");
      foreach (string field in fields) 
      {
        string name = field.Split(" ")[1];
        writeToFile(output, $"      this.{name} = {name};\n");
      }
      writeToFile(output, $"    }}\n");
      writeToFile(output, $"  }}\n");
    }
    writeToFile(output, "}");
    output.Flush();
  }
}