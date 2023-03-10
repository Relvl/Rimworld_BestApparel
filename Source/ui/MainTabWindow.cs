using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.ui.utility;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
    public class MainTabWindow : RimWorld.MainTabWindow
    {
        private const int CellPadding = 2;
        private const int NameCellWidth = 200;
        private const int CellWidth = 70;
        private const int CellHeight = 24;
        private const int HeaderHeight = 36;

        private Vector2 _scrollPosition = Vector2.zero;
        private int _lastFrameRow = -1;

        public MainTabWindow()
        {
            // super
            doCloseX = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            DataProcessor.CollectData();
        }

        public override void PreClose()
        {
            Find.WindowStack.TryRemove(typeof(FilterWindow));
            Find.WindowStack.TryRemove(typeof(ColumnsWindow));
            Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            inRect.yMin += 35f;

            TabDrawer.DrawTabs(
                inRect,
                new List<TabRecord>
                {
                    new TabRecord("BestApparel.Apparel".Translate(), () => BestApparel.Config.SelectedTab = TabId.APPAREL, BestApparel.Config.SelectedTab == TabId.APPAREL),
                    new TabRecord("BestApparel.Ranged".Translate(), () => BestApparel.Config.SelectedTab = TabId.RANGED, BestApparel.Config.SelectedTab == TabId.RANGED),
                    new TabRecord("BestApparel.Melee".Translate(), () => BestApparel.Config.SelectedTab = TabId.MELEE, BestApparel.Config.SelectedTab == TabId.MELEE),
                }
            );

            inRect.yMin += 10f;

            UIUtils.DrawButtonsRow(
                ref inRect,
                85,
                24,
                10,
                ("BestApparel.Btn.Columns", OnColumnsClick),
                ("BestApparel.Btn.Filter", OnFilterClick),
                ("BestApparel.Btn.Sorting", OnSortingClick)
            );
            UIUtils.DrawLineAtTop(ref inRect, true, 1);

            switch (BestApparel.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    RenderTabContent(inRect, DataProcessor.CachedApparels);
                    break;
                case TabId.RANGED:
                    RenderTabContent(inRect, DataProcessor.CachedRanged);
                    break;
                case TabId.MELEE:
                    break;
            }

            // Absolute positions here
            const int collectTypeWidth = 150;
            const int btnWidth = 100;

            var collectTypeRect = new Rect(windowRect.width - Margin * 2 - 10 - collectTypeWidth - btnWidth - 10, 8, collectTypeWidth, 24);
            UIUtils.RenderCheckboxLeft(
                ref collectTypeRect,
                (BestApparel.Config.UseAllThings ? "BestApparel.Control.UseAllThings" : "BestApparel.Control.UseCraftableThings").Translate(),
                BestApparel.Config.UseAllThings,
                state =>
                {
                    BestApparel.Config.UseAllThings = state;
                    DataProcessor.CollectData();
                }
            );
            TooltipHandler.TipRegion(
                collectTypeRect,
                (BestApparel.Config.UseAllThings ? "BestApparel.Control.UseAllThings.Tooltip" : "BestApparel.Control.UseCraftableThings.Tooltip").Translate()
            );

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        private void RenderTabContent(Rect inRect, IReadOnlyList<AThingContainer> thingContainers)
        {
            var firstContainer = thingContainers.FirstOrDefault();
            if (firstContainer != null)
            {
                GUI.color = BestApparel.COLOR_WHITE_A50;
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Tiny;
                var headerRect = new Rect(inRect.x + CellHeight * 2 + NameCellWidth + CellPadding * 3, inRect.y, CellWidth, HeaderHeight);
                foreach (var cell in firstContainer.CachedCells)
                {
                    headerRect.width = CellWidth;
                    Widgets.Label(headerRect, cell.DefLabel);
                    TooltipHandler.TipRegion(headerRect, cell.DefLabel);
                    headerRect.x += CellWidth + CellPadding - 2;
                    headerRect.width = 1;
                    GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
                    headerRect.x += headerRect.width;
                }

                inRect.yMin += HeaderHeight;

                headerRect.x = inRect.x;
                headerRect.y = inRect.y;
                headerRect.width = inRect.width - 16;
                headerRect.height = 1;
                GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
                inRect.yMin += headerRect.height;
            }

            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            // todo! может быть, тоже не вычислять, а получать высоту от предыдущего кадра?
            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, thingContainers.Count * (CellHeight + CellPadding));

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);

            var mouseOverAnyCell = false;

            for (var idx = 0; idx < thingContainers.Count; idx++)
            {
                var container = thingContainers[idx];
                var elementRect = new Rect(0, CellHeight * idx, inRect.width, CellHeight);

                var cellRect = new Rect(elementRect.x, elementRect.y, CellHeight, CellHeight);

                // i
                Widgets.InfoCardButtonCentered(cellRect, container.DefaultThing);

                // back row click = open info window
                if (Prefs.DevMode && Widgets.ButtonInvisible(elementRect))
                {
                    Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
                    Find.WindowStack.Add(new ThingInfoWindow(this, container.DefaultThing));
                }

                // Icon
                cellRect.x += CellHeight + CellPadding;
                Widgets.ThingIcon(cellRect, container.DefaultThing);

                // Label
                Text.Font = GameFont.Tiny;
                cellRect.x += CellHeight + CellPadding;
                cellRect.width = NameCellWidth;
                Widgets.Label(cellRect, container.DefaultThing.def.label);
                Text.Font = GameFont.Small;

                if (Prefs.DevMode)
                {
                    TooltipHandler.TipRegion(cellRect, $"Total sorting weight: {container.CachedSortingWeight}");
                }

                // todo захватить иконку в тултип
                var tip = container.DefaultThing.LabelNoParenthesisCap.AsTipTitle() + //
                          GenLabel.LabelExtras(container.DefaultThing, 1, true, true) +
                          "\n\n" +
                          container.DefaultThing.DescriptionDetailed;
                TooltipHandler.TipRegion(cellRect, tip);

                // Columns
                cellRect.x += cellRect.width + CellPadding;
                cellRect.width = CellWidth;

                for (var cellIdx = 0; cellIdx < container.CachedCells.Length; cellIdx++)
                {
                    var cell = container.CachedCells[cellIdx];

                    if (_lastFrameRow == cellIdx)
                    {
                        GUI.DrawTexture(cellRect, TexUI.HighlightTex);
                    }

                    if (Mouse.IsOver(cellRect))
                    {
                        _lastFrameRow = cellIdx;
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
                    cellRect.x += /*todo config? auto-calc?*/ cellRect.width + CellPadding - 1;
                }

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (idx < thingContainers.Count - 1)
                {
                    UIUtils.DrawLineFull(BestApparel.COLOR_WHITE_A20, CellHeight * (idx + 1), inRect.width);
                }
            }

            if (!mouseOverAnyCell)
            {
                _lastFrameRow = -1;
            }

            Text.Anchor = TextAnchor.UpperLeft;

            // todo! x-scrollable

            Widgets.EndScrollView();
        }

        private void OnFilterClick()
        {
            Find.WindowStack.TryRemove(typeof(FilterWindow));
            Find.WindowStack.Add(new FilterWindow(this));
        }

        private void OnSortingClick()
        {
            Find.WindowStack.TryRemove(typeof(SortWindow));
            Find.WindowStack.Add(new SortWindow(this));
        }

        private void OnIgnoredClick()
        {
        }

        private void OnColumnsClick()
        {
            Find.WindowStack.TryRemove(typeof(ColumnsWindow));
            Find.WindowStack.Add(new ColumnsWindow(this));
        }
    }
}