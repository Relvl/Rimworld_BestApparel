using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui;

public static class UIUtils
{
    public static Color ColorLinkHover = new(0.58f, 0.58f, 1f);
    public static Color ColorWhiteA20 = new(1f, 1f, 1f, 0.2f);
    public static Color ColorWhiteA50 = new(1f, 1f, 1f, 0.5f);

    public static void DrawLineAtTop(ref Rect inRect, bool usesScroll = true, int bottomMargin = 10)
    {
        DrawLineFull(ColorWhiteA20, inRect.y, inRect.width - /*scrollbar width*/(usesScroll ? 16 : 0));
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

    public static void DrawButtonsRowLTR(ref Rect inRect, params (TranslationCache.E, Action)[] buttons)
    {
        var r = new Rect(0, inRect.y, 0, 24);
        foreach (var (label, action) in buttons)
        {
            r.width = label.Size.x + 16;
            if (Widgets.ButtonText(r, label.Text)) action();
            r.x += r.width + 10;
        }
    }

    public static void DrawButtonsRowRight(ref Rect inRect, params (TranslationCache.E, Action, bool)[] buttons)
    {
        var r = new Rect(inRect.xMax, inRect.y, 0, 24);
        foreach (var (label, action, show) in buttons)
        {
            if (!show) continue;
            r.width = label.Size.x + 16;
            r.x -= r.width + 10;
            if (Widgets.ButtonText(r, label.Text)) action();
            if (label.Tooltip != "") TooltipHandler.TipRegion(r, label.Tooltip);
        }
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

    public static float RenderUtilityGrid<T>(ref Rect inRect, int columnCount, float rowHeight, List<T> elements, Action<T, Rect> renderElement)
    {
        var inRectStartsAt = inRect.yMin;
        if (elements.Count == 0) return inRect.yMin - inRectStartsAt;

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

            renderElement(elements[idx], cellRect);
        }

        inRect.yMin += (int)Math.Ceiling(elements.Count / (float)columnCount) * (rowHeight + padding);
        inRect.yMin += 16;

        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        return inRect.yMin - inRectStartsAt;
    }

    public static void Link(Rect btnRect, string text, Color color, Action onClick)
    {
        GUI.color = color;
        if (Mouse.IsOver(btnRect))
        {
            GUI.color = ColorLinkHover;
        }

        Widgets.Label(btnRect, text);

        if (Widgets.ButtonInvisible(btnRect))
        {
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
            onClick();
        }
    }
}