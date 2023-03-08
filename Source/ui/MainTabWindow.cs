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

        public readonly ConfigElement Config = new ConfigElement(); // todo loading from file

        private Vector2 _scrollPosition = Vector2.zero;

        public MainTabWindow()
        {
            // super
            doCloseX = true;
            // this
            Config.Load();
        }

        public override void PreOpen()
        {
            base.PreOpen();
            DoUpdate();
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
                    new TabRecord("BestApparel.Apparel".Translate(), () => Config.SelectedTab = TabId.APPAREL, Config.SelectedTab == TabId.APPAREL),
                    new TabRecord("BestApparel.Ranged".Translate(), () => Config.SelectedTab = TabId.RANGED, Config.SelectedTab == TabId.RANGED),
                    new TabRecord("BestApparel.Melee".Translate(), () => Config.SelectedTab = TabId.MELEE, Config.SelectedTab == TabId.MELEE),
                }
            );

            inRect.yMin += 10f;

            switch (Config.SelectedTab)
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
            const int btnWidth = 100;
            Rect btnRect = new Rect(windowRect.width - btnWidth - WINDOW_BORDER - 10, 4, btnWidth, 32);
            if (Widgets.ButtonText(btnRect, "BestApparel.Profiles".Translate()))
            {
                Log.Message($"windowRect: ({windowRect.x}, {windowRect.y}, {windowRect.width}, {windowRect.height}) ");
                Log.Message($"inRect: ({inRect.x}, {inRect.y}, {inRect.width}, {inRect.height}) ");
            }
        }

        private void DoUpdate()
        {
            ApparelThing.ClearThingDefs();

            Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>()
                .SelectMany(it => it.def.AllRecipes)
                .Where(it => it.AvailableNow && it.ProducedThingDef != null)
                .Select(it => it.ProducedThingDef)
                .Distinct()
                .ToList()
                .ForEach(
                    thingDef =>
                    {
                        TryToAddRangedDef(thingDef);
                        TryToAddMeleeDef(thingDef);

                        ApparelThing.TryToAddThingDef(thingDef);
                    }
                );

            TryToFinalyzeRangedDefs();
            TryFinalyzeMeleeDefs();

            ApparelThing.FinalyzeThingDefs();

            Resort();
        }

        public void Resort()
        {
            ProcessApparels();
        }

        private void OnFilterClick()
        {
            Find.WindowStack.TryRemove(typeof(FilterWindow));
            Find.WindowStack.Add(new FilterWindow(this));
        }

        private void OnSortingClick()
        {
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