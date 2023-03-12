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
        // todo def.apparel.developmentalStageFilter
        
        float heightCounter = 0;
        const int rowHeight = 20;
        foreach (var (defs, label, feature) in Parent.DataProcessor.GetFilterData(BestApparel.Config.SelectedTab))
        {
            heightCounter += UIUtils.RenderUtilityGrid(
                ref inRect,
                label,
                3,
                rowHeight,
                defs.ToList(),
                (def, cellRect) =>
                {
                    var defName = def.defName;
                    var defLabel = def.label;
                    cellRect.width = Text.CalcSize(defLabel).x + rowHeight + 6;

                    var chkRect = new Rect(cellRect.x, cellRect.y, rowHeight, rowHeight);
                    var isMouseOver = Mouse.IsOver(cellRect);

                    cellRect.xMin += 4;

                    var chkState = feature.GetState(defName);
                    if (Widgets.ButtonInvisible(cellRect))
                    {
                        chkState = feature.Toggle(defName);
                    }

                    Widgets.CheckboxMulti(chkRect, chkState);

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
        }

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultFilters();
}