using System.Collections.Generic;
using BestApparel.data;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui
{
    public class FilterWindow : AUtilityWindow
    {
        public FilterWindow(MainTabWindow parent) : base(parent)
        {
        }

        public override void DoWindowContents(Rect inRect)
        {
            switch (Parent.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    UIUtils.RenderCheckboxes(ref inRect, "BestApparel.Label.LayerList", ApparelThing.Layers, Parent.Config.EnabledLayers, Parent.Config.DisabledLayers);
                    UIUtils.RenderCheckboxes(ref inRect, "BestApparel.Label.BodyPartList", ApparelThing.BodyParts, Parent.Config.EnabledBodyParts, Parent.Config.DisabledBodyParts);
                    // todo! kid/adult
                    break;
            }

            RenderBottom(ref inRect);
        }
    }
}