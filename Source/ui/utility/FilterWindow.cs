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

            switch (Config.Instance.SelectedTab)
            {
                case TabId.APPAREL:
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.LayerList",
                        ThingContainerApparel.Layers,
                        Config.Instance.EnabledLayers,
                        Config.Instance.DisabledLayers
                    );
                    heightCounter += UIUtils.RenderCheckboxes(
                        ref inRect,
                        "BestApparel.Label.BodyPartList",
                        ThingContainerApparel.BodyParts,
                        Config.Instance.EnabledBodyParts,
                        Config.Instance.DisabledBodyParts
                    );
                    // todo! kid/adult
                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => Config.Instance.RestoreDefaultFilters();
    }
}