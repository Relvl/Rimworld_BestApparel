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

            // Table header
            const int cellPadding = 2;
            const int nameCellWidth = 200;
            const int cellWidth = 70;
            const int cellHeight = 24;

            if (DataProcessor.CachedApparels.Length > 0)
            {
                var apparel = DataProcessor.CachedApparels.FirstOrDefault();
                if (apparel != null)
                {
                    Text.Font = GameFont.Tiny;
                    var headerRect = new Rect(inRect.x + cellHeight * 2 + nameCellWidth + cellPadding * 3, inRect.y, cellWidth, cellHeight);
                    foreach (var cell in apparel.CachedCells)
                    {
                        GUI.color = BestApparel.COLOR_WHITE_A50;
                        Text.Anchor = TextAnchor.MiddleCenter;

                        headerRect.width = cellWidth;

                        Widgets.Label(headerRect, cell.DefLabel);
                        TooltipHandler.TipRegion(headerRect, cell.DefLabel);

                        headerRect.x += cellWidth + cellPadding;
                        headerRect.width = 1;

                        GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
                    }

                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                    inRect.yMin += cellHeight + cellPadding;
                }
            }

            // todo! может быть, тоже не вычислять, а получать высоту от предыдущего кадра?
            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, DataProcessor.CachedApparels.Length * cellHeight);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            var mouseOverAnyCell = false;

            for (var idx = 0; idx < DataProcessor.CachedApparels.Length; idx++)
            {
                var apparel = DataProcessor.CachedApparels[idx];
                var elementRect = new Rect(0, cellHeight * idx, inRect.width, cellHeight);
                var cellRect = new Rect(elementRect.x, elementRect.y, cellHeight, cellHeight);

                // i
                Widgets.InfoCardButtonCentered(cellRect, apparel.DefaultThing);

                // back row click = open info window
                if (Prefs.DevMode && Widgets.ButtonInvisible(elementRect))
                {
                    Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
                    Find.WindowStack.Add(new ThingInfoWindow(this, apparel.DefaultThing));
                }

                // Icon
                cellRect.x += cellHeight + cellPadding;
                Widgets.ThingIcon(cellRect, apparel.DefaultThing);

                // Label
                Text.Font = GameFont.Tiny;
                cellRect.x += cellHeight + cellPadding;
                cellRect.width = nameCellWidth;
                Widgets.Label(cellRect, apparel.DefaultThing.def.label);
                Text.Font = GameFont.Small;

                if (Prefs.DevMode)
                {
                    TooltipHandler.TipRegion(cellRect, $"Total sorting weight: {apparel.CachedSortingWeight}");
                }

                // todo захватить иконку в тултип
                TooltipHandler.TipRegion(cellRect, apparel.DefaultThing.Label);

                // Columns
                cellRect.x += cellRect.width + cellPadding;
                cellRect.width = cellWidth;

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
                    cellRect.x += /*todo config? auto-calc?*/ cellRect.width + cellPadding;
                }

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (idx < DataProcessor.CachedApparels.Length - 1)
                {
                    UIUtils.DrawLineFull(BestApparel.COLOR_WHITE_A20, cellHeight * (idx + 1), inRect.width);
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