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

        var defs = BestApparel.Config.GetColumnsFor(BestApparel.Config.SelectedTab).OrderBy(it => it).ToList();

        heightCounter += UIUtils.RenderUtilityGrid( //
            ref inRect,
            TranslationCache.LabelSorting,
            2,
            30,
            defs,
            (defName, cellRect) =>
            {
                var defLabel = Parent.DataProcessor.GetStatProcessors(BestApparel.Config.SelectedTab).FirstOrDefault(p => p.GetDefName() == defName)?.GetDefLabel();
                if (defLabel == null) return;

                var oldValue = BestApparel.Config.GetSortingFor(BestApparel.Config.SelectedTab)[defName];
                var value = oldValue;
                Widgets.HorizontalSlider(cellRect, ref value, new FloatRange(-Config.MaxSortingWeight, Config.MaxSortingWeight), $"{defLabel}: {value}", 1);
                if (Math.Abs(oldValue - value) > 0.1)
                {
                    BestApparel.Config.GetSortingFor(BestApparel.Config.SelectedTab)[defName] = value;
                }
            }
        );

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultSortingFor();
}