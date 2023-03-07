using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    public abstract class AUtilityWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(650, 500);

        protected readonly MainTabWindow Parent;

        protected AUtilityWindow(MainTabWindow parent)
        {
            // base
            doCloseX = true;
            draggable = true;
            // this
            Parent = parent;
        }

        public override void PreClose()
        {
            Parent.Resort();
        }

        protected void RenderBottom(ref Rect inRect)
        {
            const int btnHeight = 24;
            const int btnWidth = 120;
            var btnRect = new Rect(0, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
            if (Widgets.ButtonText(btnRect, "BestApparel.Btn.Resort".Translate()))
            {
                Parent.Resort();
            }

            btnRect.x += btnWidth + 10;
            btnRect.width = inRect.width - btnRect.x;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = ModEntrance.COLOR_WHITE_A20;
            Widgets.Label(btnRect, "BestApparel.Btn.Resort.Additional".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            btnRect = new Rect(windowRect.width - Margin * 2 - btnWidth, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
            if (Widgets.ButtonText(btnRect, "BestApparel.Btn.Defaults".Translate()))
            {
                Parent.Config.Defaults();
                Parent.Resort();
            }
        }
    }
}