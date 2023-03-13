using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public static Rect TranslateToGUI(this Rect rect) => new(GUIUtility.GUIToScreenPoint(rect.position), rect.size);

    public static void ReplaceWith<T>(this List<T> col, IEnumerable<T> newCol)
    {
        col.Clear();
        col.AddRange(newCol);
    }
}