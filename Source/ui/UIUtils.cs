using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel.ui;

public static class UIUtils
{
    public static void DrawLineAtTop(ref Rect inRect, bool usesScroll = true, int bottomMargin = 10)
    {
        DrawLineFull(BestApparel.ColorWhiteA20, inRect.y, inRect.width - /*scrollbar width*/(usesScroll ? 16 : 0));
        if (bottomMargin > 0)
        {
            inRect.yMin += bottomMargin;
        }
    }

    public static void DrawLineFull(Color color, float y, float width)
    {
        GUI.color = color;
        Widgets.DrawLineHorizontal(0, y, width);
        GUI.color = Color.white;
    }

    public static void DrawButtonsRow(ref Rect inRect, int btnWidth, int btnHeight, int margin, params (string, Action)[] buttons)
    {
        var r = new Rect(0, inRect.y, 0, 0);
        foreach (var (label, action) in buttons)
        {
            DrawButtonRightOffset(ref r, label, action, btnWidth, btnHeight);
        }

        inRect.yMin += btnHeight + margin;
    }

    public static void DrawButtonRightOffset(ref Rect r, string label, Action onClick, int btnWidth = 85, int btnHeight = 24)
    {
        r.width = btnWidth;
        r.height = btnHeight;
        if (Widgets.ButtonText(r, label.Translate())) onClick();
        r.x += btnWidth + 10;
    }

    public static void RenderCheckboxLeft(ref Rect inRect, string label, bool state, Action<bool> onStateChanged, float maxWidth = 0)
    {
        var r = new Rect(inRect.x, inRect.y, maxWidth == 0 ? inRect.width : maxWidth, 24);
        if (Widgets.ButtonInvisible(r))
        {
            onStateChanged(!state);
        }

        Widgets.CheckboxDraw(inRect.x, inRect.y, state, false);
        r.x += r.height + 2;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(r, label);
        Text.Anchor = TextAnchor.UpperLeft;
    }

    public static float RenderUtilityGrid<T>(ref Rect inRect, string label, int columnCount, int rowHeight, List<T> elements, Action<T, Rect> action)
    {
        var inRectStartsAt = inRect.yMin;
        if (elements.Count == 0) return inRect.yMin - inRectStartsAt;

        RenderUtilityHeader(ref inRect, label);
        Text.Anchor = TextAnchor.MiddleLeft;
        Text.Font = GameFont.Tiny;

        var colWidth = inRect.width / columnCount - 2;
        var cellRect = new Rect(inRect.x, inRect.y, 0, 16);
        const int padding = 5;

        for (var idx = 0; idx < elements.Count; idx++)
        {
            var colIdx = idx % columnCount;
            var rowIdx = idx / columnCount;

            cellRect.x = (colWidth + padding) * colIdx;
            cellRect.width = colWidth;
            cellRect.y = inRect.y + (rowHeight + padding) * rowIdx;
            cellRect.height = rowHeight;

            GUI.color = Color.white;

            action(elements[idx], cellRect);
        }

        inRect.yMin += (int)Math.Ceiling(elements.Count / (float)columnCount) * (rowHeight + padding);
        inRect.yMin += 16;

        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        return inRect.yMin - inRectStartsAt;
    }

    public static void RenderUtilityHeader(ref Rect inRect, string label)
    {
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        var labelTranslated = label.Translate();
        var labelWidth = Text.CalcSize(labelTranslated.RawText);
        var labelRect = new Rect(inRect.x, inRect.y, labelWidth.x, 20);
        Widgets.Label(labelRect, labelTranslated);
        TooltipHandler.TipRegion(labelRect, $"{label}.Tooltip".Translate());

        inRect.yMin += 36;
    }
}