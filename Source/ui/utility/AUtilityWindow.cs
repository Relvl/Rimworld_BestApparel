using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public abstract class AUtilityWindow : Window
{
    public override Vector2 InitialSize => new(650, 500);

    protected virtual bool UseBottomButtons => true;
    protected virtual bool UseSearch => false;

    protected virtual float RowHeight => 20;

    protected readonly IThingTabRenderer Parent;
    protected string SearchString;

    private Vector2 _mainScrollPosition = Vector2.zero;
    private float _lastFrameScrollHeight;

    protected AUtilityWindow(IThingTabRenderer parent)
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
            _lastFrameScrollHeight += RenderBottom(ref inRect);
        }

        if (UseSearch)
        {
            const int searchWidth = 100;
            var r = new Rect(windowRect.width - Margin * 2 - 28 * 3 - searchWidth, windowRect.height - Margin * 2 - 24, 24, 24);
            GUI.DrawTexture(r, TexButton.Search);
            GUI.SetNextControlName($"UI.BestApparel.{GetType().Name}.Search");
            r.x += 28;
            r.width = searchWidth;
            var str = Widgets.TextField(r, SearchString, 15);
            SearchString = str;
            // todo! clear search button
        }
    }

    protected abstract float DoWindowContentsInner(ref Rect inRect);

    protected abstract void OnResetClick();

    private float RenderBottom(ref Rect inRect)
    {
        const float btnHeight = 24;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        var btnRect = new Rect(windowRect.width - Margin * 2 - btnHeight, windowRect.height - Margin * 2 - btnHeight, btnHeight, btnHeight);
        TooltipHandler.TipRegion(btnRect, TranslationCache.BtnDefaults.Text);
        if (Widgets.ButtonImage(btnRect, TexButton.Reload))
        {
            OnResetClick();
            Parent.UpdateFilter();
        }

        return btnHeight;
    }

    protected static float RenderTitle(ref Rect inRect, TranslationCache.E label, Action<Rect> renderRightSide = null)
    {
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
        GUI.color = UIUtils.ColorLinkHover;

        var labelRect = new Rect(inRect.x, inRect.y, label.Size.x, 20);
        Widgets.Label(labelRect, label.Text);
        TooltipHandler.TipRegion(labelRect, label.Tooltip);

        renderRightSide?.Invoke(new Rect(inRect.x + labelRect.width, labelRect.y, inRect.width - labelRect.width - 16, 20));

        GUI.color = Color.white;
        inRect.yMin += 36;
        return 36;
    }

    protected float RenderTitle(ref Rect inRect, TranslationCache.E label, List<Def> defs) =>
        RenderTitle(
            ref inRect,
            label,
            buttonLineRect =>
            {
                var btnRect = new Rect(buttonLineRect.x + buttonLineRect.width, buttonLineRect.y, 0, 20);

                var textNone = "BestApparel.Btn.Utility.None".Translate();
                btnRect.xMin -= Text.CalcSize(textNone).x;
                UIUtils.Link(
                    btnRect,
                    textNone,
                    Color.red,
                    () =>
                    {
                        foreach (var def in defs) BestApparel.Config.SetFilter(Parent.GetTabId(), def.defName, MultiCheckboxState.Off);
                        Parent.UpdateFilter();
                    }
                );

                btnRect.width = 0;
                btnRect.x -= 10;

                var textReset = "BestApparel.Btn.Utility.Default".Translate();
                btnRect.xMin -= Text.CalcSize(textReset).x;
                UIUtils.Link(
                    btnRect,
                    textReset,
                    Color.yellow,
                    () =>
                    {
                        foreach (var def in defs) BestApparel.Config.SetFilter(Parent.GetTabId(), def.defName, MultiCheckboxState.Partial);
                        Parent.UpdateFilter();
                    }
                );

                btnRect.width = 0;
                btnRect.x -= 10;

                var textAll = "BestApparel.Btn.Utility.All".Translate();
                btnRect.xMin -= Text.CalcSize(textAll).x;
                UIUtils.Link(
                    btnRect,
                    textAll,
                    Color.green,
                    () =>
                    {
                        foreach (var def in defs) BestApparel.Config.SetFilter(Parent.GetTabId(), def.defName, MultiCheckboxState.On);
                        Parent.UpdateFilter();
                    }
                );
            }
        );

    protected float RenderTitle(ref Rect inRect, TranslationCache.E label, IEnumerable<string> defs, Action<bool> onSet) =>
        RenderTitle(
            ref inRect,
            label,
            buttonLineRect =>
            {
                var btnRect = new Rect(buttonLineRect.x + buttonLineRect.width, buttonLineRect.y, 0, 20);

                var textNone = "BestApparel.Btn.Utility.None".Translate();
                btnRect.xMin -= Text.CalcSize(textNone).x;
                UIUtils.Link(
                    btnRect,
                    textNone,
                    Color.red,
                    () =>
                    {
                        onSet(false);
                        Parent.UpdateFilter();
                    }
                );

                btnRect.width = 0;
                btnRect.x -= 10;

                var textReset = "BestApparel.Btn.Utility.All".Translate();
                btnRect.xMin -= Text.CalcSize(textReset).x;
                UIUtils.Link(
                    btnRect,
                    textReset,
                    Color.green,
                    () =>
                    {
                        onSet(true);
                        Parent.UpdateFilter();
                    }
                );
            }
        );
}