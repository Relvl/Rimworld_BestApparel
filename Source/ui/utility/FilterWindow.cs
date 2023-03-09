using BestApparel.data;
using UnityEngine;

namespace BestApparel.ui.utility
{
    public class FilterWindow : AUtilityWindow
    {
        public FilterWindow(MainTabWindow parent) : base(parent)
        {
        }

        protected override float DoWindowContentsInner(ref Rect inRect)
        {
            float heightCounter = 0;

            switch (BestApparel.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.LayerList",
                        ThingContainerApparel.Layers,
                        BestApparel.Config.EnabledLayers,
                        BestApparel.Config.DisabledLayers
                    );
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.BodyPartList",
                        ThingContainerApparel.BodyParts,
                        BestApparel.Config.EnabledBodyParts,
                        BestApparel.Config.DisabledBodyParts
                    );
                    // todo! kid/adult
                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => BestApparel.Config.RestoreDefaultFilters();
    }
}