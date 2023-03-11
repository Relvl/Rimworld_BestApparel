using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class SortWindow : AUtilityWindow
{
    public SortWindow(MainTabWindow parent) : base(parent)
    {
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        float heightCounter = 0;

        switch (BestApparel.Config.SelectedTab)
        {
            case TabId.Apparel:
                heightCounter += RenderApparelSorting(ref inRect);
                break;
        }

        return heightCounter;
    }

    protected override void OnResetClick()
    {
        BestApparel.Config.RestoreDefaultSortingFor();
    }

    private float RenderApparelSorting(ref Rect inRect)
    {
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        var inRectStartsAt = inRect.yMin;
        var labelRect = new Rect(inRect.x, inRect.y, inRect.width, 20);
        Widgets.Label(labelRect, "BestApparel.Label.Sorting".Translate());
        TooltipHandler.TipRegion(labelRect, "BestApparel.Label.Sorting.Tooltip".Translate());
        inRect.yMin += 26;

        if (Parent.DataProcessor.GetTable(TabId.Apparel).Count == 0) return inRect.yMin - inRectStartsAt;

        var r = new Rect(inRect.x, inRect.y, 0, 16);
        Text.Anchor = TextAnchor.MiddleLeft;
        const int sliderHeight = 30;
        const int columnCount = 2;
        var colWidth = inRect.width / columnCount - 2;

        var columns = BestApparel.Config.Apparel.Columns.OrderBy(it => it).ToList();
        if (columns.Count == 0) return inRect.yMin - inRectStartsAt;
        for (var idx = 0; idx < columns.Count; idx++)
        {
            var statDefName = columns[idx];

            var defLabel = Parent.DataProcessor.GetStatProcessors(TabId.Apparel).FirstOrDefault(p => p.GetDefName() == statDefName)?.GetDefLabel();
            if (defLabel == null) continue;


            var colIdx = idx % columnCount;
            var rowIdx = idx / columnCount;

            // Label
            r.x = colWidth * colIdx + 2 * colIdx;
            r.width = colWidth;
            r.y = inRect.y + sliderHeight * rowIdx + 2 * rowIdx;
            r.height = sliderHeight;

            var oldValue = BestApparel.Config.GetSortingFor(TabId.Apparel)[statDefName];
            var value = oldValue;
            Widgets.HorizontalSlider(r, ref value, new FloatRange(-Config.MaxSortingWeight, Config.MaxSortingWeight), $"{defLabel}: {value}", 1);
            if (Math.Abs(oldValue - value) > 0.1)
            {
                BestApparel.Config.GetSortingFor(TabId.Apparel)[statDefName] = value;
            }
        }

        inRect.yMin += columns.Count / columnCount * (sliderHeight + 2) + 20;

        return inRect.yMin - inRectStartsAt;
    }
}