using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    // ReSharper disable once UnusedType.Global -> /Defs/MainWindow.xml
    public partial class MainTabWindowBestApparel
    {
        private void RenderApparelTab(Rect inRect)
        {
            /*  [Where...] [Layers...] [Slots...] [Sorting...] [Ignored...] [Colons...]
             *  {{{{{{{{{{{{{{{{{{{{{
             *  LIST
             *  }}}}}}}}}}}}}}}}}}}}}
             */
            const int btnWidth = 85;
            var r = new Rect(0, inRect.y, btnWidth, 24);

            if (Widgets.ButtonText(r, "BestApparel.Btn.Where".Translate())) OnWhereClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Layers".Translate())) OnLayersLick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Slots".Translate())) OnSlotsClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Sorting".Translate())) OnSortingClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Ignored".Translate())) OnIgnoredClick();
            r.x += btnWidth + 10;

            if (Widgets.ButtonText(r, "BestApparel.Btn.Columns".Translate())) OnColumnsClick();
            r.x += btnWidth + 10;

            inRect.yMin += 34;

            Utils.DrawLineFull(ModBestApparel.COLOR_WHITE_A20, inRect.y, inRect.width - /*scrollbar width*/16);

            inRect.yMin += 10;

            // region TABLE

            // todo! count lines
            var sortedThings = _thingList.Where(it => it.thing.def.IsApparel).ToArray();

            var innerScrolledRect = new Rect(0, 0, inRect.width - /*scrollbar width*/16,
                sortedThings.Length * LIST_ELEMENT_HEIGHT);

            Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);
            Text.Anchor = TextAnchor.MiddleLeft;

            for (var i = 0; i < sortedThings.Length; i++)
            {
                // todo вытащить в метод

                var rThing = sortedThings[i];
                var elementRect = new Rect(0, LIST_ELEMENT_HEIGHT * i, inRect.width, LIST_ELEMENT_HEIGHT);
                var cellRect = new Rect(elementRect.x, elementRect.y, LIST_ELEMENT_HEIGHT, LIST_ELEMENT_HEIGHT);

                // i
                Widgets.InfoCardButtonCentered(cellRect, rThing.thing);
                
                // Icon
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                Widgets.ThingIcon(cellRect, rThing.thing);
                // Label
                cellRect.x += LIST_ELEMENT_HEIGHT + 4;
                cellRect.width = 200;
                Widgets.Label(cellRect, rThing.thing.def.label);
                // todo захватить иконку
                TooltipHandler.TipRegion(cellRect, rThing.thing.Label);

                // todo columns
                
                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (i < sortedThings.Length - 1)
                {
                    Utils.DrawLineFull(ModBestApparel.COLOR_WHITE_A20, LIST_ELEMENT_HEIGHT * i + LIST_ELEMENT_HEIGHT,
                        inRect.width);
                }
            }

            Widgets.EndScrollView();
            // endregion
        }

        private void OnWhereClick()
        {
        }

        private void OnLayersLick()
        {
        }

        private void OnSlotsClick()
        {
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