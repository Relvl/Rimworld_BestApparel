using BestApparel.data;
using UnityEngine;

namespace BestApparel.ui
{
    public class FilterWindow : AUtilityWindow
    {
        public FilterWindow(MainTabWindow parent) : base(parent)
        {
        }

        protected override float DoWindowContentsInner(ref Rect inRect)
        {
            float heightCounter = 0;

            switch (Parent.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.LayerList",
                        ApparelThing.Layers,
                        Parent.Config.EnabledLayers,
                        Parent.Config.DisabledLayers
                    );
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.BodyPartList",
                        ApparelThing.BodyParts,
                        Parent.Config.EnabledBodyParts,
                        Parent.Config.DisabledBodyParts
                    );
                    // todo! kid/adult
                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => Parent.Config.RestoreDefaultFilters();
    }
}