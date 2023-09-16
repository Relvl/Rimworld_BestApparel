using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui.utility;

public class FilterWindow : AUtilityWindow
{
    protected override bool UseSearch => true;

    public FilterWindow(IThingTabRenderer parent) : base(parent)
    {
        Log.Warning("Created window");
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        // todo def.apparel.developmentalStageFilter

        float heightCounter = 0;
        var lSearch = SearchString.ToLower();

        foreach (var (defs, label, category) in Parent.GetFilterData())
        {
            var defsAsList = defs.Where(def => lSearch == "" || def.defName.ToLower().Contains(lSearch) || def.label.ToLower().Contains(lSearch)).ToList();
            if (defsAsList.Count == 0) continue;
            heightCounter += RenderFilterTitle(ref inRect, label, defsAsList, category);
            heightCounter += UIUtils.RenderUtilityGrid(ref inRect, 3, RowHeight, defsAsList, (def, rect) => RenderElement(def, rect, category));
        }

        return heightCounter;
    }

    private void RenderElement(Def def, Rect cellRect, string category)
    {
        var defName = def.defName;
        var defLabel = def.label;
        cellRect.width = Text.CalcSize(defLabel).x + RowHeight + 6;

        var chkRect = new Rect(cellRect.x, cellRect.y, RowHeight, RowHeight);
        var isMouseOver = Mouse.IsOver(cellRect);

        cellRect.xMin += 4;

        var chkState = BestApparel.Config.GetFilter(Parent.GetTabId(), category, defName);
        if (Widgets.ButtonInvisible(cellRect))
        {
            chkState = BestApparel.Config.ToggleFilter(Parent.GetTabId(), category, defName);
            Parent.UpdateFilter();
        }

        var tex = chkState switch
        {
            MultiCheckboxState.On => Widgets.CheckboxOnTex,
            MultiCheckboxState.Off => Widgets.CheckboxOffTex,
            _ => Widgets.CheckboxPartialTex
        };
        Widgets.ButtonImage(chkRect, tex, true);
        MouseoverSounds.DoRegion(chkRect);

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

    protected override void OnResetClick() => BestApparel.Config.ClearFilters(Parent.GetTabId());
}