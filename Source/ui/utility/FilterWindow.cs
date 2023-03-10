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
                    heightCounter += UIUtils.RenderFeatureSwitches( //
                        ref inRect,
                        "BestApparel.Label.LayerList",
                        ThingContainerApparel.Layers,
                        BestApparel.Config.ApparelLayers
                    );
                    heightCounter += UIUtils.RenderFeatureSwitches( //
                        ref inRect,
                        "BestApparel.Label.BodyPartList",
                        ThingContainerApparel.BodyParts,
                        BestApparel.Config.ApparelBodyParts
                    );
                    heightCounter += UIUtils.RenderFeatureSwitches( //
                        ref inRect,
                        "BestApparel.Label.ThingCategories",
                        ThingContainerApparel.Categories,
                        BestApparel.Config.ApparelCategories
                    );
                    // todo! kid/adult
                    break;

                case TabId.RANGED:
                    heightCounter += UIUtils.RenderFeatureSwitches( //
                        ref inRect,
                        "BestApparel.Label.WeaponTypes",
                        ThingContainerRanged.WeaponClasses,
                        BestApparel.Config.RangedTypes
                    );
                    heightCounter += UIUtils.RenderFeatureSwitches( //
                        ref inRect,
                        "BestApparel.Label.ThingCategories",
                        ThingContainerRanged.Categories,
                        BestApparel.Config.RangedCategories
                    );

                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => BestApparel.Config.RestoreDefaultFilters();
    }
}