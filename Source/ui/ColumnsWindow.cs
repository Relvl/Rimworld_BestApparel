using BestApparel.data;
using UnityEngine;

namespace BestApparel.ui
{
    public class ColumnsWindow : AUtilityWindow
    {
        public ColumnsWindow(MainTabWindow parent) : base(parent)
        {
        }

        public override void DoWindowContents(Rect inRect)
        {
            switch (Parent.Config.SelectedTab)
            {
                case TabId.APPAREL:
                    RenderApparelColumns(ref inRect);
                    break;
            }

            RenderBottom(ref inRect);
        }

        private void RenderApparelColumns(ref Rect inRect)
        {
            UIUtils.RenderCheckboxes(ref inRect, "BestApparel.Label.Columns", ApparelThing.Stats, Parent.Config.SelectedColumns[TabId.APPAREL], null, 2);
        }
    }
}