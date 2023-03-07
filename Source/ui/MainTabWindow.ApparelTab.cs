using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
    public partial class MainTabWindow
    {
        public readonly List<ApparelThing> Apparels = new List<ApparelThing>();
        public readonly List<ApparelLayerDef> ApparelLayers = new List<ApparelLayerDef>();
        public readonly List<StuffCategoryDef> ApparelStuffs = new List<StuffCategoryDef>();
        public readonly List<BodyPartGroupDef> ApparelBodyParts = new List<BodyPartGroupDef>();

        private void RenderApparelTab(Rect inRect)
        {
            /*  [Filter...] [Sorting...] [Ignored...] [Colons...]
             *  {{{{{{{{{{{{{{{{{{{{{
             *  LIST
             *  }}}}}}}}}}}}}}}}}}}}}
             */
            const int btnWidth = 85;
            var r = new Rect(0, inRect.y, btnWidth, 24);

            if (Widgets.ButtonText(r, "BestApparel.Btn.Filter".Translate())) OnFilterClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Sorting".Translate())) OnSortingClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Ignored".Translate())) OnIgnoredClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Columns".Translate())) OnColumnsClick();
            r.x += btnWidth + 10;

            inRect.yMin += 34;

            Utils.DrawLineFull(ModEntrance.COLOR_WHITE_A20, inRect.y, inRect.width - /*scrollbar width*/16);

            inRect.yMin += 10;

            // region TABLE

            // todo! cache this!
            var sortedThings = Apparels
                .Where(
                    it =>
                    {
                        // if have any 'ON' state - the thing should contain ALL of it
                        if (Config.EnabledLayers.Count > 0)
                        {
                            if (!it.Def.apparel.layers.All(l => Config.EnabledLayers.Contains(l.defName)))
                            {
                                return false;
                            }
                        }

                        // if have any 'OFF' state - the thing should not contain it
                        if (Config.DisabledLayers.Count > 0)
                        {
                            if (it.Def.apparel.layers.Any(l => Config.DisabledLayers.Contains(l.defName)))
                            {
                                return false;
                            }
                        }

                        // if have any 'ON' state - the thing should contain ANY of it
                        if (Config.EnabledBodyParts.Count > 0)
                        {
                            if (!it.Def.apparel.bodyPartGroups.Any(l => Config.EnabledBodyParts.Contains(l.defName)))
                            {
                                return false;
                            }
                        }

                        // if have any 'OFF' state - the thing should not contain it
                        if (Config.DisabledBodyParts.Count > 0)
                        {
                            if (it.Def.apparel.bodyPartGroups.Any(l => Config.DisabledBodyParts.Contains(l.defName)))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                )
                .ToArray();

            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, sortedThings.Length * LIST_ELEMENT_HEIGHT);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            for (var i = 0; i < sortedThings.Length; i++)
            {
                // todo вытащить в метод

                var rThing = sortedThings[i];
                var elementRect = new Rect(0, LIST_ELEMENT_HEIGHT * i, inRect.width, LIST_ELEMENT_HEIGHT);
                var cellRect = new Rect(elementRect.x, elementRect.y, LIST_ELEMENT_HEIGHT, LIST_ELEMENT_HEIGHT);

                // i
                Widgets.InfoCardButtonCentered(cellRect, rThing.DefaultThing);

                // Icon
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                Widgets.ThingIcon(cellRect, rThing.DefaultThing);
                // Label
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                cellRect.width = 200;
                Widgets.Label(cellRect, rThing.DefaultThing.def.label);
                // todo захватить иконку
                TooltipHandler.TipRegion(cellRect, rThing.DefaultThing.Label);

                // todo columns

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (i < sortedThings.Length - 1)
                {
                    Utils.DrawLineFull(
                        ModEntrance.COLOR_WHITE_A20,
                        LIST_ELEMENT_HEIGHT * i + LIST_ELEMENT_HEIGHT,
                        inRect.width
                    );
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();
            // endregion
        }

        private void TryToAddApparelDef(ThingDef thingDef)
        {
            if (thingDef.IsApparel)
            {
                Apparels.Add(new ApparelThing(thingDef));
            }
        }

        private void TryFinalyzeApparelDefs()
        {
            ApparelLayers.Clear();
            ApparelLayers.AddRange(
                Apparels
                    .SelectMany(it => it.Def.apparel.layers)
                    .Where(it => it != null)
                    .Distinct()
            );
            ApparelStuffs.Clear();
            ApparelStuffs.AddRange(
                Apparels
                    .Where(it => it.Def.stuffProps?.categories != null)
                    .SelectMany(it => it.Def.stuffProps?.categories)
                    .Distinct()
            );
            ApparelBodyParts.Clear();
            ApparelBodyParts.AddRange(
                Apparels
                    .SelectMany(it => it.Def.apparel.bodyPartGroups)
                    .Where(it => it != null)
                    .Distinct()
            );
        }

        private void OnSortingClick()
        {
        }

        private void OnIgnoredClick()
        {
        }

        private void OnColumnsClick()
        {
        }
    }
}