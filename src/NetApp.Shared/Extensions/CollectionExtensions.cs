namespace NetApp.Extensions;

public static class CollectionExtensions
{
  public static bool IsSupersetOf<T>(this IEnumerable<T> mainCollection, params T[] subCollection)
  {
    var otherHashSet = new HashSet<T>(subCollection);
    return mainCollection.All(otherHashSet.Contains);
  }
}
