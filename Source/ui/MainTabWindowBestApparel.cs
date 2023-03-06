using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.logic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global -> /Defs/MainWindow.xml
    public partial class MainTabWindowBestApparel : MainTabWindow
    {
        private static bool _isCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

        private readonly BestApparelConfigElement _config = new BestApparelConfigElement(); // todo loading from file
        private readonly BestApparelEvents _events = new BestApparelEvents();
        private readonly List<ComparableThing> _thingList = new List<ComparableThing>();

        private Vector2 _scrollPosition = Vector2.zero;
        private bool _isDirty;
        private int _listUpdateNext = 0;

        public MainTabWindowBestApparel()
        {
            // super
            doCloseX = true;
            doCloseButton = false;
            closeOnClickedOutside = true;
            // this
            _config.Load();
            // finally
            _isDirty = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (_isDirty) DoUpdate();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            inRect.yMin += 35f;

            TabDrawer.DrawTabs(inRect, new List<TabRecord>
            {
                new TabRecord("BestApparel.Apparel".Translate(), () => _config.SelectedTab = BestApparelTab.APPAREL,
                    _config.SelectedTab == BestApparelTab.APPAREL),
                new TabRecord("BestApparel.Ranged".Translate(), () => _config.SelectedTab = BestApparelTab.RANGED,
                    _config.SelectedTab == BestApparelTab.RANGED),
                new TabRecord("BestApparel.Melee".Translate(), () => _config.SelectedTab = BestApparelTab.MELEE,
                    _config.SelectedTab == BestApparelTab.MELEE),
            });

            inRect.yMin += 10f;

            switch (_config.SelectedTab)
            {
                case BestApparelTab.APPAREL:
                    RenderApparelTab(inRect);
                    break;
                case BestApparelTab.RANGED:
                    RenderRangedTab(inRect);
                    break;
                case BestApparelTab.MELEE:
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
            var tempList = new List<ComparableThing>();
            var buildingList = Find.CurrentMap.listerBuildings.allBuildingsColonist;
            foreach (var building in buildingList)
            {
                if (!(building is Building_WorkTable workTable)) continue;
                var recipeList = workTable.def.AllRecipes;

                foreach (var recipe in recipeList)
                {
                    if (!recipe.AvailableNow) continue;
                    var thingDef = recipe.ProducedThingDef;
                    if (thingDef == null) continue;
                    if (!thingDef.IsWeapon && !thingDef.IsApparel) continue;

                    // todo! деструктуризация по материалу
                    // todo! вычислить лучший материал по выбранным параметрам сортировки
                    var comparableThing = CoverThing(
                        thingDef.MadeFromStuff
                            ? ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef))
                            : ThingMaker.MakeThing(thingDef)
                    );
                    if (comparableThing != null)
                    {
                        tempList.Add(comparableThing);
                    }
                }
            }

            _thingList.Clear();
            _thingList.AddRange(tempList);
            _isDirty = false;
        }

        private static ComparableThing CoverThing(Thing thing)
        {
            if (thing.def.IsWeapon)
            {
                return new WeaponThing(thing);
            }
            else if (thing.def.IsApparel)
            {
                return new ApparelThing(thing);
            }
            else return null;
        }
    }
}