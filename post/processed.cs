using Newtonsoft.Json;
using tairasoul.pdsl.visitors.returnTypes;

namespace tairasoul.pdsl.postprocessed;

class PneumaticraftJsonObject(int version, Widget[] widgets)
{
  public int version = version;
  public Widget[] widgets = widgets;
  public string GetJson() 
  {
    return JsonConvert.SerializeObject(this);
  }
}