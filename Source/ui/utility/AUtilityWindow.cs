using System;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public abstract class AUtilityWindow : Window
{
    public override Vector2 InitialSize => new(650, 500);

    protected virtual bool UseBottomButtons => true;
    protected virtual bool UseSearch => false;

    protected readonly MainTabWindow Parent;
    protected string SearchString;

    private Vector2 _mainScrollPosition = Vector2.zero;
    private float _lastFrameScrollHeight;

    protected AUtilityWindow(MainTabWindow parent)
    {
        // base
        doCloseX = true;
        draggable = true;
        // this
        Parent = parent;
    }

    public override void PreOpen()
    {
        base.PreOpen();
        SearchString = "";
    }

    public override void DoWindowContents(Rect inRect)
    {
        var scrolledRect = new Rect(0, 0, inRect.width - 16, _lastFrameScrollHeight);
        _lastFrameScrollHeight = 0;
        inRect.height -= 40; // bottom buttons

        Widgets.BeginScrollView(inRect, ref _mainScrollPosition, scrolledRect);
        _lastFrameScrollHeight += DoWindowContentsInner(ref inRect);
        Widgets.EndScrollView();

        if (UseBottomButtons)
        {
            _lastFrameScrollHeight += RenderBottom(ref inRect, OnResetClick);
        }

        if (UseSearch)
        {
            const int searchWidth = 100;
            var r = new Rect(windowRect.width - Margin * 2 - 48 - searchWidth, 0, 24, 24);
            GUI.DrawTexture(r, TexButton.Search);
            GUI.SetNextControlName($"UI.BestApparel.{GetType().Name}.Search");
            r.x += 28;
            r.width = searchWidth;
            var str = Widgets.TextField(r, SearchString, 15);
            SearchString = str;
        }
    }

    protected abstract float DoWindowContentsInner(ref Rect inRect);

    protected abstract void OnResetClick();

    public override void PreClose()
    {
        Parent.DataProcessor.Rebuild();
    }

    private float RenderBottom(ref Rect inRect, Action onResetClick)
    {
        const float btnHeight = 24;
        const int btnWidth = 120;
        var btnRect = new Rect(0, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
        if (Widgets.ButtonText(btnRect, TranslationCache.BtnResort.Text))
        {
            Parent.DataProcessor.Rebuild();
        }

        btnRect.x += btnWidth + 10;
        btnRect.width = inRect.width - btnRect.x;
        Text.Anchor = TextAnchor.MiddleLeft;
        GUI.color = BestApparel.ColorWhiteA20;
        Widgets.Label(btnRect, TranslationCache.BtnResort.Tooltip);
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        btnRect = new Rect(windowRect.width - Margin * 2 - btnWidth, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
        if (Widgets.ButtonText(btnRect, TranslationCache.BtnDefaults.Text))
        {
            onResetClick();
            Parent.DataProcessor.Rebuild();
        }

        return btnHeight;
    }
}