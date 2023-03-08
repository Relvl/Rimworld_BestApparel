using System.Linq;
using BestApparel.data;
using UnityEngine;

namespace BestApparel.ui.utility
{
    public class ColumnsWindow : AUtilityWindow
    {
        public ColumnsWindow(MainTabWindow parent) : base(parent)
        {
        }

        protected override float DoWindowContentsInner(ref Rect inRect)
        {
            float heightCounter = 0;
            switch (Config.Instance.SelectedTab)
            {
                case TabId.APPAREL:
                    heightCounter += RenderApparelColumns(ref inRect);
                    break;
            }

            return heightCounter;
        }

        protected override void OnResetClick() => Config.Instance.RestoreDefaultColumns();

        private float RenderApparelColumns(ref Rect inRect)
        {
            return UIUtils.RenderCheckboxes(
                ref inRect,
                "BestApparel.Label.Columns",
                ThingContainerApparel.StatProcessors.Select(it => it.GetStatDef()).ToList(),
                Config.Instance.SelectedColumns[TabId.APPAREL],
                null,
                2
            );
        }
    }
}