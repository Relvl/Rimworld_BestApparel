using System.Linq;
using UnityEngine;

namespace BestApparel.ui.utility;

public class ColumnsWindow : AUtilityWindow
{
    protected override bool UseSearch => true;

    public ColumnsWindow(MainTabWindow parent) : base(parent)
    {
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        float heightCounter = 0;
        switch (BestApparel.Config.SelectedTab)
        {
            case TabId.Apparel:
                heightCounter += RenderApparelColumns(ref inRect);
                break;
            case TabId.Ranged:
                heightCounter += RenderRangedColumns(ref inRect);
                break;
        }

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultColumns();

    private float RenderApparelColumns(ref Rect inRect)
    {
        return UIUtils.RenderCheckboxes(
            ref inRect,
            "BestApparel.Label.Columns",
            Parent.DataProcessor.GetStatProcessors(TabId.Apparel)
                .Where(proc => SearchString == "" || proc.GetDefName().Contains(SearchString) || proc.GetDefLabel().Contains(SearchString))
                .ToList(),
            BestApparel.Config.Apparel.Columns,
            null,
            2
        );
    }

    private float RenderRangedColumns(ref Rect inRect)
    {
        return UIUtils.RenderCheckboxes(
            ref inRect,
            "BestApparel.Label.Columns",
            Parent.DataProcessor.GetStatProcessors(TabId.Ranged)
                .Where(proc => SearchString == "" || proc.GetDefName().Contains(SearchString) || proc.GetDefLabel().Contains(SearchString))
                .ToList(),
            BestApparel.Config.Ranged.Columns,
            null,
            2
        );
    }
}