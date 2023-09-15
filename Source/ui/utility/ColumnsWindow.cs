using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui.utility;

public class ColumnsWindow : AUtilityWindow
{
    protected override bool UseSearch => true;

    public ColumnsWindow(IThingTabRenderer parent) : base(parent)
    {
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        float heightCounter = 0;
        var lSearch = SearchString.ToLower();
        var processors = Parent.GetColumnData()
            .Where(proc => lSearch == "" || proc.GetDefName().ToLower().Contains(lSearch) || proc.GetDefLabel().ToLower().Contains(lSearch))
            .ToList();

        if (processors.Count == 0) return heightCounter;

        heightCounter += RenderColumnsTitle(ref inRect, TranslationCache.LabelColumns, processors.Select(p => p.GetDefName()));

        heightCounter += UIUtils.RenderUtilityGrid(
            ref inRect,
            2,
            20,
            processors,
            (processor, cellRect) =>
            {
                var defName = processor.GetDefName();
                var defLabel = processor.GetDefLabel();
                cellRect.width = Text.CalcSize(defLabel).x + RowHeight + 6;

                var chkRect = new Rect(cellRect.x, cellRect.y, RowHeight, RowHeight);
                var isMouseOver = Mouse.IsOver(cellRect);

                cellRect.xMin += 4;

                var chkState = BestApparel.Config.GetColumn(Parent.GetTabId(), defName);
                if (Widgets.ButtonInvisible(cellRect))
                {
                    BestApparel.Config.SetColumn(Parent.GetTabId(), defName, !chkState);
                    if (chkState)
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                    else
                        SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                    Parent.UpdateSort();
                }

                Widgets.CheckboxDraw(chkRect.x, chkRect.y, chkState, false, RowHeight);

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
        );

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.Config.ClearColumns(Parent.GetTabId());
}