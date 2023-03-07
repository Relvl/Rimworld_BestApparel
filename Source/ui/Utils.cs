using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    public static class Utils
    {
        public static void DrawLineFull(Color color, float y, float width)
        {
            GUI.color = color;
            Widgets.DrawLineHorizontal(0, y, width);
            GUI.color = Color.white;
        }
    }
}