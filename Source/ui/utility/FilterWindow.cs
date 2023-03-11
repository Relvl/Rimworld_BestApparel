using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class FilterWindow : AUtilityWindow
{
    public FilterWindow(MainTabWindow parent) : base(parent)
    {
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        float heightCounter = 0;
        foreach (var (defs, label, config) in Parent.DataProcessor.GetFilterData(BestApparel.Config.SelectedTab))
        {
            heightCounter += UIUtils.RenderFeatureSwitches(ref inRect, label, defs.ToList(), config);
        }

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultFilters();
}