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
            T? result;
            bag.TryTake(out result);

            // Check if result is null
            if (result == null || !predicate(result))
            {
                // If result is null or predicate is false, add the item back to the bag
                if (result != null)
                {
                    bag.Add(result);
                }
            }
        }
    }
}