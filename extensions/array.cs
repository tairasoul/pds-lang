namespace tairasoul.pdsl.extensions;

static class ArrayExtensions 
{
  public static bool Includes<T>(this T[] @object, T value) 
  {
    foreach (T obj in @object) {
      if (EqualityComparer<T>.Default.Equals(obj, value)) return true;
    }
    return false;
  }
  
  public static string ConcatString<T>(this T[] @object, Func<T, bool, string> joiner) 
  {
    string res = joiner(@object[0], true);
    for (int i = 1; i < @object.Length; i++) 
    {
      res += joiner(@object[i], false);
    }
    return res;
  }
}