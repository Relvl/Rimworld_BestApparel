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

            var innerScrolledRect = new Rect(0, 0, inRect.width - 16, DataProcessor.CachedApparels.Length * LIST_ELEMENT_HEIGHT);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            for (var i = 0; i < DataProcessor.CachedApparels.Length; i++)
            {
                var rThing = DataProcessor.CachedApparels[i];
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


                foreach (var cell in rThing.CachedCells)
                {
                    if (cell.None)
                    {
                        GUI.color = ModEntrance.COLOR_WHITE_A20;
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

                        if (Prefs.DevMode)
                        {
                            TooltipHandler.TipRegion(cellRect, $"Stat defName: {cell.DefName}");
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

                if (i < DataProcessor.CachedApparels.Length - 1)
                {
                    UIUtils.DrawLineFull(ModEntrance.COLOR_WHITE_A20, LIST_ELEMENT_HEIGHT * i + LIST_ELEMENT_HEIGHT, inRect.width);
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();
            // endregion
        }
    }
}