using BestApparel.data;
using UnityEngine;

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