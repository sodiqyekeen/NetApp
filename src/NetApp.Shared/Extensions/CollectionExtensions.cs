namespace NetApp.Extensions;

public static class CollectionExtensions
{
    public static bool ContainsAll<T>(this IEnumerable<T> mainCollection, params T[] subCollection)
    {
        if (mainCollection == null) throw new ArgumentNullException(nameof(mainCollection));
        if (subCollection == null) throw new ArgumentNullException(nameof(subCollection));
        var isSubset = !subCollection.Except(mainCollection).Any();
        return isSubset;
    }

}
