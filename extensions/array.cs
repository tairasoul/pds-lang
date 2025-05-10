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
}