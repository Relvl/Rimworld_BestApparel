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
    public partial class MainTabWindow : RimWorld.MainTabWindow
    {
        private static bool _isCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

        private Vector2 _scrollPosition = Vector2.zero;

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

            switch (BestApparel.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    RenderApparelTab(inRect);
                    break;
                case TabId.RANGED:
                    RenderRangedTab(inRect);
                    break;
                case TabId.MELEE:
                    RenderMeleeTab(inRect);
                    break;
            }

            // Absolute positions here
            const int searchTypeWidth = 150;
            const int btnWidth = 100;

            var searchTypeRect = new Rect(windowRect.width - WINDOW_BORDER - 10 - searchTypeWidth - btnWidth - 10, 8, searchTypeWidth, 24);
            UIUtils.RenderCheckboxLeft(
                ref searchTypeRect,
                (BestApparel.Config.UseAllThings ? "BestApparel.Control.UseAllThings" : "BestApparel.Control.UseCraftableThings").Translate(),
                BestApparel.Config.UseAllThings,
                state =>
                {
                    BestApparel.Config.UseAllThings = state;
                    DataProcessor.CollectData();
                }
            );
            TooltipHandler.TipRegion(
                searchTypeRect,
                (BestApparel.Config.UseAllThings ? "BestApparel.Control.UseAllThings.Tooltip" : "BestApparel.Control.UseCraftableThings.Tooltip").Translate()
            );

            var addControlRect = new Rect(windowRect.width - btnWidth - WINDOW_BORDER - 10, 4, btnWidth, 32);
            if (Widgets.ButtonText(addControlRect, "BestApparel.Profiles".Translate()))
            {
                Log.Message($"windowRect: ({windowRect.x}, {windowRect.y}, {windowRect.width}, {windowRect.height}) ");
                Log.Message($"inRect: ({inRect.x}, {inRect.y}, {inRect.width}, {inRect.height}) ");
            }
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