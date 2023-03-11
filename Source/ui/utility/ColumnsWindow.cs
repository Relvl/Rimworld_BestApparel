using System.Linq;
using UnityEngine;
using Verse;

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
        var defs = Parent.DataProcessor.GetStatProcessors(BestApparel.Config.SelectedTab)
            .Where(proc => SearchString == "" || proc.GetDefName().Contains(SearchString) || proc.GetDefLabel().Contains(SearchString))
            .ToList();
        const int rowHeight = 20;
        var enabled = BestApparel.Config.GetColumnsFor(BestApparel.Config.SelectedTab);

        heightCounter += UIUtils.RenderUtilityGrid(
            ref inRect,
            "BestApparel.Label.Columns",
            2,
            20,
            defs,
            (processor, cellRect) =>
            {
                var defName = processor.GetDefName();
                var defLabel = processor.GetDefLabel();
                cellRect.width = Text.CalcSize(defLabel).x + rowHeight + 6;

                var chkRect = new Rect(cellRect.x, cellRect.y, rowHeight, rowHeight);
                var isMouseOver = Mouse.IsOver(cellRect);

                cellRect.xMin += 4;

                var chkState = enabled.Contains(defName);
                if (Widgets.ButtonInvisible(cellRect))
                {
                    if (chkState) enabled.Remove(defName);
                    else enabled.Add(defName);
                }

                Widgets.CheckboxDraw(chkRect.x, chkRect.y, chkState, false, rowHeight);

                if (isMouseOver)
                {
                    GUI.DrawTexture(chkRect, TexUI.HighlightTex);
                    GUI.color = Color.yellow;
                    if (Prefs.DevMode)
                    {
                        TooltipHandler.TipRegion(cellRect, $"defName: {defName}");
                    }
                }

                cellRect.x += rowHeight + 2;
                Widgets.Label(cellRect, defLabel);
            }
        );

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultColumns();
}