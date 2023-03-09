using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.stat_processor;
using BestApparel.ui.utility;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
    public partial class MainTabWindow
    {
        private int _apparelLastFrameRow = -1;

        private void RenderApparelTab(Rect inRect)
        {
            UIUtils.DrawButtonsRow(
                ref inRect,
                85,
                24,
                10,
                ("BestApparel.Btn.Columns", OnColumnsClick),
                ("BestApparel.Btn.Filter", OnFilterClick),
                ("BestApparel.Btn.Sorting", OnSortingClick)
                //,("BestApparel.Btn.Ignored", OnIgnoredClick)
            );

            UIUtils.DrawLineAtTop(ref inRect);

            // region TABLE

            // todo! может быть, тоже не вычислять, а получать высоту от предыдущего кадра?
            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, DataProcessor.CachedApparels.Length * LIST_ELEMENT_HEIGHT);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            var mouseOverAnyCell = false;

            for (var idx = 0; idx < DataProcessor.CachedApparels.Length; idx++)
            {
                var apparel = DataProcessor.CachedApparels[idx];
                var elementRect = new Rect(0, LIST_ELEMENT_HEIGHT * idx, inRect.width, LIST_ELEMENT_HEIGHT);
                var cellRect = new Rect(elementRect.x, elementRect.y, LIST_ELEMENT_HEIGHT, LIST_ELEMENT_HEIGHT);

                // i
                Widgets.InfoCardButtonCentered(cellRect, apparel.DefaultThing);

                // back row click = open info window
                if (Prefs.DevMode && Widgets.ButtonInvisible(elementRect))
                {
                    Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
                    Find.WindowStack.Add(new ThingInfoWindow(this, apparel.DefaultThing));
                }

                // Icon
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                Widgets.ThingIcon(cellRect, apparel.DefaultThing);

                // Label
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                cellRect.width = 200;
                Widgets.Label(cellRect, apparel.DefaultThing.def.label);

                if (Prefs.DevMode)
                {
                    TooltipHandler.TipRegion(cellRect, $"Total sorting weight: {apparel.CachedSortingWeight}");
                }

                // todo захватить иконку в тултип
                TooltipHandler.TipRegion(cellRect, apparel.DefaultThing.Label);

                // Columns
                cellRect.x += cellRect.width + 2;
                cellRect.width = 70;

                for (var cellIdx = 0; cellIdx < apparel.CachedCells.Length; cellIdx++)
                {
                    var cell = apparel.CachedCells[cellIdx];

                    if (_apparelLastFrameRow == cellIdx)
                    {
                        GUI.DrawTexture(cellRect, TexUI.HighlightTex);
                    }

                    if (Mouse.IsOver(cellRect))
                    {
                        _apparelLastFrameRow = cellIdx;
                        mouseOverAnyCell = true;
                    }

                    if (cell.IsEmpty)
                    {
                        GUI.color = BestApparel.COLOR_WHITE_A20;
                        Widgets.Label(cellRect, cell.Value);
                        GUI.color = Color.white;
                    }
                    else
                    {
                        Widgets.Label(cellRect, cell.Value);
                        foreach (var tooltip in cell.Tooltips)
                        {
                            TooltipHandler.TipRegion(cellRect, tooltip);
                        }
                    }

                    // offset to the right
                    cellRect.x += /*todo config? auto-calc?*/ cellRect.width + 2;
                }

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (idx < DataProcessor.CachedApparels.Length - 1)
                {
                    UIUtils.DrawLineFull(BestApparel.COLOR_WHITE_A20, LIST_ELEMENT_HEIGHT * idx + LIST_ELEMENT_HEIGHT, inRect.width);
                }
            }

            if (!mouseOverAnyCell)
            {
                _apparelLastFrameRow = -1;
            }

            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();
            // endregion
        }
    }
}