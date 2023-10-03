using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class SortWindow : AUtilityWindow
{
    protected override float RowHeight => 30;

    public SortWindow(IThingTabRenderer parent) : base(parent)
    {
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        float heightCounter = 0;

        var columns = BestApparel.GetTabConfig(Parent.GetTabId()).Columns.GetColumns().OrderBy(it => it).ToList();

        heightCounter += RenderTitle(ref inRect, TranslationCache.LabelSorting);

        heightCounter += UIUtils.RenderUtilityGrid( //
            ref inRect,
            2,
            RowHeight,
            columns,
            (defName, cellRect) =>
            {
                var defLabel = Parent.GetColumnData().FirstOrDefault(p => p.GetDefName() == defName)?.GetDefLabel();
                if (defLabel == null) return;

                var oldValue = BestApparel.GetTabConfig(Parent.GetTabId()).Sorting.GetSorting(defName);
                var value = oldValue;
                Widgets.HorizontalSlider(cellRect, ref value, new FloatRange(-Config.MaxSortingWeight, Config.MaxSortingWeight), $"{defLabel}: {value}", 1);
                if (Math.Abs(oldValue - value) > 0.1)
                {
                    BestApparel.GetTabConfig(Parent.GetTabId()).Sorting.SetSorting(defName, value);
                    Parent.UpdateSort();
                }
            }
        );

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.GetTabConfig(Parent.GetTabId()).Sorting.Clear();
}