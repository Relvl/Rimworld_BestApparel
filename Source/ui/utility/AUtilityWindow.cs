using System;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility
{
    public abstract class AUtilityWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(650, 500);
        public virtual bool UseBottomButtons => true;

        protected readonly MainTabWindow Parent;
        private Vector2 _mainScrollPosition = Vector2.zero;
        private float _lastFrameScrollHeight = 0f;

        protected AUtilityWindow(MainTabWindow parent)
        {
            // base
            doCloseX = true;
            draggable = true;
            // this
            Parent = parent;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var scrolledRect = new Rect(0, 0, inRect.width - 16, _lastFrameScrollHeight);
            _lastFrameScrollHeight = 0;
            inRect.height -= 40; // bottom buttons

            Widgets.BeginScrollView(inRect, ref _mainScrollPosition, scrolledRect);
            _lastFrameScrollHeight += DoWindowContentsInner(ref inRect);
            Widgets.EndScrollView();

            _lastFrameScrollHeight += RenderBottom(ref inRect, OnResetClick);
        }

        protected abstract float DoWindowContentsInner(ref Rect inRect);

        protected abstract void OnResetClick();

        public override void PreClose()
        {
            Parent.Resort();
        }

        private float RenderBottom(ref Rect inRect, Action onResetClick)
        {
            const float btnHeight = 24;
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
                onResetClick();
                Parent.Resort();
            }

            return btnHeight;
        }
    }
}