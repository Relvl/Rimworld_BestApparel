using System.Collections.Generic;
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
        foreach (var (defs, label, feature) in Parent.DataProcessor.GetFilterData(BestApparel.Config.SelectedTab))
        {
            var defsAsList = defs.ToList();
            var featuredDefs = defsAsList.Select(def => (def, feature)).ToList();
            if (featuredDefs.Count == 0) continue;

            heightCounter += RenderTitle(ref inRect, label, defsAsList, feature);
            heightCounter += UIUtils.RenderUtilityGrid(ref inRect, 3, RowHeight, featuredDefs, RenderElement);
        }

        return heightCounter;
    }

    private void RenderElement((Def, FeatureEnableDisable) element, Rect cellRect)
    {
        var defName = element.Item1.defName;
        var defLabel = element.Item1.label;
        cellRect.width = Text.CalcSize(defLabel).x + RowHeight + 6;

        var chkRect = new Rect(cellRect.x, cellRect.y, RowHeight, RowHeight);
        var isMouseOver = Mouse.IsOver(cellRect);

        cellRect.xMin += 4;

        var chkState = element.Item2.GetState(defName);
        if (Widgets.ButtonInvisible(cellRect))
        {
            chkState = element.Item2.Toggle(defName);
            Parent.DataProcessor.Rebuild();
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

        cellRect.x += RowHeight + 2;
        Widgets.Label(cellRect, defLabel);
    }

    protected override void OnResetClick() => BestApparel.Config.RestoreDefaultFilters();
}