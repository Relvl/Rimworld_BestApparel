using System;
using System.Collections.Generic;
using System.Linq;

namespace BestApparel;

public static class Extensions
{
    public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        for (var i = 0; i < collection.Count; i++)
        {
            var element = collection.ElementAt(i);
            if (!predicate(element)) continue;
            collection.Remove(element);
            i--;
        }
    }
}