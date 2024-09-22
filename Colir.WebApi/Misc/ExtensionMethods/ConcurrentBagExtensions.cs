using System.Collections.Concurrent;

namespace Colir.Misc.ExtensionMethods;

public static class ConcurrentBagExtensions
{
    /// <summary>
    /// Removes all items from the bag that satisfy the predicate
    /// </summary>
    public static void RemoveWhere<T>(this ConcurrentBag<T> bag, Func<T, bool> predicate)
    {
        while (bag.Count > 0)
        {
            T result;
            bag.TryTake(out result!);

            // If the predicate is false, add the item back to the bag
            if (!predicate(result))
            {
                bag.Add(result);
            }
        }
    }
}