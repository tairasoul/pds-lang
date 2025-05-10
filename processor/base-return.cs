using Newtonsoft.Json;
using tairasoul.pdsl.pieces;

namespace tairasoul.pdsl.visitors.returnTypes;

class Widget(string type, Coordinate pos) 
{
  public string type = type;
  public Coordinate pos = pos;
  
  [JsonExtensionData]
  public Dictionary<string, object> ExtraProperties { get; set; } = [];
}

class VisitorReturn(Coordinate setLast, Widget[] widgets)
{
  public Coordinate setLast = setLast;
  public Widget[] widgets = widgets;
}