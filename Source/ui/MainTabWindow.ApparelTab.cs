using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.stat_processor;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
    public partial class MainTabWindow
    {
        private ApparelThing[] _cachedApparels = { };

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

            UIUtils.DrawLineFull(ModEntrance.COLOR_WHITE_A20, inRect.y, inRect.width - /*scrollbar width*/16);

            inRect.yMin += 10;

            // region TABLE

            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, _cachedApparels.Length * LIST_ELEMENT_HEIGHT);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            for (var i = 0; i < _cachedApparels.Length; i++)
            {
                var rThing = _cachedApparels[i];
                var elementRect = new Rect(0, LIST_ELEMENT_HEIGHT * i, inRect.width, LIST_ELEMENT_HEIGHT);
                var cellRect = new Rect(elementRect.x, elementRect.y, LIST_ELEMENT_HEIGHT, LIST_ELEMENT_HEIGHT);

                // i
                Widgets.InfoCardButtonCentered(cellRect, rThing.DefaultThing);

                // back row click = open info window
                if (Widgets.ButtonInvisible(elementRect))
                {
                    Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
                    Find.WindowStack.Add(new ThingInfoWindow(this, rThing.DefaultThing));
                }

                // Icon
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                Widgets.ThingIcon(cellRect, rThing.DefaultThing);
                // Label
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                cellRect.width = 200;
                Widgets.Label(cellRect, rThing.DefaultThing.def.label);
                // todo захватить иконку
                TooltipHandler.TipRegion(cellRect, rThing.DefaultThing.Label);

                // Columns
                // todo! order like in sorting weight
                cellRect.x += cellRect.width + 2;
                cellRect.width = 70;
                Config.SelectedColumns[TabId.APPAREL]
                    .ForEach(
                        colId =>
                        {
                            var proc = ApparelThing.StatProcessors.FirstOrDefault(it => it.GetStatDef().defName == colId);
                            if (proc == null) return;
                            var value = proc.GetStatValue(rThing.DefaultThing);
                            if (value != 0)
                            {
                                var formattedValue = AStatProcessor.GetStatValueFormatted(proc.GetStatDef(), value, true);
                                Widgets.Label(cellRect, formattedValue);
                                TooltipHandler.TipRegion(cellRect, $"{proc.GetStatDef().label}: {formattedValue}");
                                if (Prefs.DevMode)
                                {
                                    TooltipHandler.TipRegion(cellRect, $"Stat defName: {colId}");
                                }
                            }
                            else
                            {
                                GUI.color = ModEntrance.COLOR_WHITE_A20;
                                Widgets.Label(cellRect, "---");
                                GUI.color = Color.white;
                            }

                            // offset to the right
                            cellRect.x += /*todo config? auto-calc?*/ cellRect.width + 2;
                        }
                    );

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (i < _cachedApparels.Length - 1)
                {
                    UIUtils.DrawLineFull(ModEntrance.COLOR_WHITE_A20, LIST_ELEMENT_HEIGHT * i + LIST_ELEMENT_HEIGHT, inRect.width);
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();
            // endregion
        }

        /** Filters and sorts the apparel for renderer */
        private void ProcessApparels()
        {
            _cachedApparels = ApparelThing.AllApparels
                // Filter: Layer, BodyPart, Material
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
                        // endregion ================= FILTER

                        return true;
                    }
                )
                // Sort: Default
                .OrderBy(it => it.DefaultThing.HitPoints)
                .ThenBy(it => it.DefaultThing.Label)
                // Finish
                .ToArray();
        }
    }
}