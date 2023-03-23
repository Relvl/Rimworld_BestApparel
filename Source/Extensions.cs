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

    public static void ReplaceWith<T>(this List<T> col, IEnumerable<T> newCol)
    {
        col.Clear();
        col.AddRange(newCol);
    }

    public static TV ComputeIfAbsent<TK, TV>(this IDictionary<TK, TV> dict, TK key, Func<TV> producer)
    {
        var v = dict.TryGetValue(key, out var value) ? value : producer();
        dict[key] = v;
        return v;
    }
}