using System.Linq;
using BestApparel.data;
using UnityEngine;

namespace BestApparel.ui.utility
{
    public class ColumnsWindow : AUtilityWindow
    {
        protected override bool UseSearch => true;

        public ColumnsWindow(MainTabWindow parent) : base(parent)
        {
        }

        protected override float DoWindowContentsInner(ref Rect inRect)
        {
            float heightCounter = 0;
            switch (BestApparel.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    heightCounter += RenderApparelColumns(ref inRect);
                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => BestApparel.Config.RestoreDefaultColumns();

        private float RenderApparelColumns(ref Rect inRect)
        {
            return UIUtils.RenderCheckboxes(
                ref inRect,
                "BestApparel.Label.Columns",
                ThingContainerApparel.StatProcessors.Select(it => it.GetStatDef())
                    .Where(proc => SearchString == "" || proc.defName.Contains(SearchString) || proc.label.Contains(SearchString))
                    .ToList(),
                BestApparel.Config.Columns.Apparel,
                null,
                2
            );
        }
    }
}