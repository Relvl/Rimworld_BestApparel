using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui.utility;

public class ColumnsWindow(IThingTabRenderer parent) : AUtilityWindow(parent)
{
    protected override bool UseSearch => true;

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

                var chkState = BestApparel.GetTabConfig(Parent.GetTabId()).Columns.GetColumn(defName);
                if (Widgets.ButtonInvisible(cellRect))
                {
                    if (chkState)
                    {
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                    }
                    else
                    {
                        foreach (var dependant in processor.ActivateWith)
                            BestApparel.GetTabConfig(Parent.GetTabId()).Columns.SetColumn(dependant, true);
                        SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                    }

                    BestApparel.GetTabConfig(Parent.GetTabId()).Columns.SetColumn(defName, !chkState);
                    if (chkState) BestApparel.GetTabConfig(Parent.GetTabId()).Sorting.SetSorting(defName, 0);

                    Parent.UpdateSort();
                }

                Widgets.CheckboxDraw(chkRect.x, chkRect.y, chkState, false, RowHeight);

                var tooltip = $"{processor.GetDefLabel()}";
                if (processor.StatDef.description is not null)
                {
                    tooltip += $"\n\n{processor.StatDef.description}";
                }

                if (Prefs.DevMode)
                {
                    tooltip += processor.DebugTooltip();
                }

                TooltipHandler.TipRegion(cellRect, tooltip);

                if (isMouseOver)
                {
                    GUI.DrawTexture(chkRect, TexUI.HighlightTex);
                    GUI.color = Color.yellow;
                }

                cellRect.x += RowHeight + 2;
                Widgets.Label(cellRect, defLabel);
            }
        );

        return heightCounter;
    }

    protected override void OnResetClick() => BestApparel.GetTabConfig(Parent.GetTabId()).Columns.Clear();
}