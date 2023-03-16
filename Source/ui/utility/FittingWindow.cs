using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.data.impl;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class FittingWindow : Window, IReloadObserver
{
    public override Vector2 InitialSize => new(650, 500);

    private readonly MainTabWindow _parent;
    private Vector2 _scrollLeft = Vector2.zero;
    private Vector2 _scrollRight = Vector2.zero;
    private BodyPartGroupDef _selectedBodyPart;

    private readonly List<ThingContainerApparel> _worn = new();
    private float _lastFrameListHeight;
    private string _search = "";
    private readonly List<ThingContainerApparel> _apparelsFiltered = new();

    private const int CellPadding = 3;
    private const int BpCellHeight = 20;
    private const int IconSize = 24;
    private const int APCellHeight = IconSize + CellPadding * 2;
    private const float ScrollSize = 16;

    public FittingWindow(MainTabWindow parent)
    {
        resizeable = true;
        draggable = true;
        doCloseX = true;
        _parent = parent;
    }

    public override void PreOpen()
    {
        base.PreOpen();
        _worn.ReplaceWith(_parent.DataProcessor.GetAllApparels().Where(it => BestApparel.Config.FittingWorn.Contains(it.Def.defName)));
        DoSomethingChanged();
        _parent.DataProcessor.ReloadObservers.Add(this);
    }

    public override void PreClose()
    {
        _parent.DataProcessor.ReloadObservers.Remove(this);
        Config.ModInstance.WriteSettings();
    }

    public void OnDataProcessorReloaded() => DoSomethingChanged();

    private void DoSomethingChanged(bool changed = true)
    {
        if (!changed) return;
        _apparelsFiltered.ReplaceWith(
            _parent.DataProcessor.GetAllApparels()
                .Where(
                    it =>
                    {
                        if (!it.IsSearchAccept(_search)) return false;
                        if (_selectedBodyPart != null) return it.Def.apparel.bodyPartGroups.Contains(_selectedBodyPart);
                        if (_worn.Any(worn => !ApparelUtility.CanWearTogether(worn.Def, it.Def, BodyDefOf.Human))) return false;
                        return true;
                    }
                )
                .OrderByDescending(it => it.CachedSortingWeight)
                .ThenBy(it => it.Def.label)
        );
        BestApparel.Config.FittingWorn.ReplaceWith(_worn.Select(it => it.Def.defName));
    }

    public override void DoWindowContents(Rect inRect)
    {
        // window title 
        Text.Font = GameFont.Medium;
        GUI.color = Color.green;
        Widgets.Label(inRect, TranslationCache.FittingWindowTitle.Text);
        inRect.yMin += 46;

        // =================================================
        var panesHeight = inRect.height - 70;

        var leftPaneRect = new Rect(0, inRect.yMin, 300, panesHeight);

        RenderLeftPane(leftPaneRect);

        var btnRect = new Rect(0, inRect.yMax - 36, 100, IconSize);
        if (Widgets.ButtonText(btnRect, TranslationCache.FittingBtnClear.Text))
        {
            _worn.Clear();
            DoSomethingChanged();
        }

        btnRect.x += btnRect.width + CellPadding;

        if (Widgets.ButtonText(btnRect, TranslationCache.FittingBtnFromPawn.Text))
        {
            Find.WindowStack.Add(
                new FloatMenu(
                    Find.CurrentMap.mapPawns.AllPawnsSpawned //
                        .Where(it => it.IsColonist)
                        .Select(
                            pawn => new ColonistFloatMenuOption( //
                                pawn,
                                () =>
                                {
                                    _worn.ReplaceWith(pawn.apparel.WornApparel.Select(_parent.DataProcessor.GetApparelOfDef));
                                    DoSomethingChanged();
                                }
                            )
                        )
                        .Cast<FloatMenuOption>()
                        .ToList()
                )
            );
        }

        TooltipHandler.TipRegion(btnRect, TranslationCache.FittingBtnFromPawn.Tooltip);

        var rightPaneRect = new Rect(leftPaneRect.xMax + CellPadding, inRect.yMin, inRect.width - leftPaneRect.width - CellPadding * 2, panesHeight);
        RenderRightPane(rightPaneRect);

        // =================================================
    }

    private void RenderLeftPane(Rect paneRect)
    {
        var bodyParts = _parent.DataProcessor.GetApparelBodyParts().ToList();

        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var cellRightOffset = _lastFrameListHeight > paneRect.height ? ScrollSize : 0;
        var scrollInnerRect = new Rect(0, 0, paneRect.width - CellPadding * 2 - cellRightOffset, _lastFrameListHeight);

        Widgets.BeginScrollView(paneRect, ref _scrollLeft, scrollInnerRect);
        var cellRect = new Rect(CellPadding, CellPadding, scrollInnerRect.width, BpCellHeight);
        for (var bpIdx = 0; bpIdx < bodyParts.Count; bpIdx++)
        {
            var bodyPart = bodyParts[bpIdx];
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            var apparels = _worn.Where(it => it.Def.apparel?.bodyPartGroups?.Contains(bodyPart) ?? false).ToArray();

            cellRect.height = BpCellHeight + (apparels.Length > 0 ? apparels.Length * 26 : 18);

            GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (Mouse.IsOver(cellRect)) GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (_selectedBodyPart == bodyPart) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

            // body part name
            Text.Font = GameFont.Tiny;
            // todo! font if have any or not
            var typeLabelRect = new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding, cellRect.width, 16);
            Widgets.Label(typeLabelRect, bodyPart.label);

            if (apparels.Length > 1)
            {
                var removeAllRect = new Rect(
                    cellRect.xMax - TranslationCache.FittingBtnRemoveAll.Size.x - CellPadding * 2,
                    cellRect.y + CellPadding,
                    TranslationCache.FittingBtnRemoveAll.Size.x,
                    16
                );
                Text.Anchor = TextAnchor.MiddleRight;
                GUI.color = Color.red;
                if (Mouse.IsOver(removeAllRect)) GUI.color = Color.yellow;
                Widgets.Label(removeAllRect, TranslationCache.FittingBtnRemoveAll.Text);
                TooltipHandler.TipRegion(removeAllRect, TranslationCache.FittingBtnRemoveAll.Tooltip);
                Text.Anchor = TextAnchor.UpperLeft;
                if (Widgets.ButtonInvisible(removeAllRect)) OnRemoveAllClick(bodyPart);
            }

            // apparels in slot name
            if (apparels.Any())
            {
                Text.Font = GameFont.Small;
                GUI.color = BestApparel.ColorWhiteA50;

                for (var thingIdx = 0; thingIdx < apparels.Length; thingIdx++)
                {
                    var apparel = apparels[thingIdx];
                    var offset = 16 + thingIdx * 26;
                    Widgets.ThingIcon(new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding + offset, IconSize, IconSize), apparel.DefaultThing);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    var labelRect = new Rect(cellRect.x + IconSize + CellPadding * 2, cellRect.y + CellPadding + offset, scrollInnerRect.width, IconSize);
                    Widgets.Label(labelRect, apparel.Def.label);
                    TooltipHandler.TipRegion(labelRect, apparel.DefaultTooltip);

                    var removeRect = new Rect(cellRect.xMax - CellPadding + cellRightOffset - ScrollSize - IconSize, cellRect.y + CellPadding + offset, IconSize, IconSize);
                    GUI.color = Color.red;
                    if (Mouse.IsOver(removeRect)) GUI.color = Color.yellow;
                    if (Widgets.ButtonText(removeRect, "-"))
                    {
                        _worn.RemoveIf(containerApparel => containerApparel == apparel);
                        DoSomethingChanged();
                    }

                    TooltipHandler.TipRegion(removeRect, TranslationCache.FittingBtnRemove.Text);
                }
            }
            else
            {
                Text.Font = GameFont.Tiny;
                GUI.color = BestApparel.ColorWhiteA50;
                Widgets.Label(new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding + 16, scrollInnerRect.width, IconSize), TranslationCache.FittingLabelSelectApparel.Text);
            }

            if (Widgets.ButtonInvisible(cellRect))
            {
                _selectedBodyPart = _selectedBodyPart == null ? bodyPart : null;
                DoSomethingChanged();
            }

            cellRect.y += cellRect.height + CellPadding;
        }

        _lastFrameListHeight = cellRect.y;

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.EndScrollView();
    }

    private void RenderRightPane(Rect paneRect)
    {
        var searchRect = new Rect(paneRect.x, paneRect.y, IconSize, IconSize);
        GUI.DrawTexture(searchRect, TexButton.Search);
        GUI.SetNextControlName("UI.BestApparel.Fitting.Search");
        searchRect.x += IconSize + CellPadding;
        searchRect.width = paneRect.width - searchRect.width * 2 - CellPadding * 2;
        var newSearch = Widgets.TextField(searchRect, _search, 15);
        var changed = _search != newSearch;
        searchRect.xMin += searchRect.width + CellPadding;
        searchRect.width = IconSize;
        if (Widgets.ButtonImage(searchRect.ContractedBy(4), TexButton.CloseXSmall)) newSearch = "";
        _search = newSearch;
        DoSomethingChanged(changed);

        paneRect.yMin += IconSize + CellPadding;

        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var apparelsFiltered = _apparelsFiltered.ToList();

        var scrolledHeight = (APCellHeight + CellPadding) * apparelsFiltered.Count + CellPadding * 2;
        var cellRightOffset = scrolledHeight > paneRect.height ? ScrollSize : 0;
        var scrollInnerRect = new Rect(0, 0, paneRect.width - CellPadding * 2 - cellRightOffset, scrolledHeight);

        Widgets.Label(new Rect(300, paneRect.height + 200, 100, 20), $"scrolly: {_scrollRight.y}");

        Widgets.BeginScrollView(paneRect, ref _scrollRight, scrollInnerRect);
        var cellRect = new Rect(CellPadding, CellPadding, scrollInnerRect.width, APCellHeight);
        foreach (var apparel in apparelsFiltered)
        {
            // FPS optimisition - do not render items that not in the wievbox
            var isInViewRange = _scrollRight.y < cellRect.y + cellRect.height * 2 && _scrollRight.y + paneRect.height > cellRect.yMax - cellRect.height;
            if (isInViewRange)
            {
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;

                GUI.DrawTexture(cellRect, TexUI.HighlightTex);
                if (Mouse.IsOver(cellRect)) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

                // Thing icon
                Widgets.ThingIcon(new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding, IconSize, IconSize), apparel.DefaultThing);

                // Button [+]
                var btnRect = new Rect(cellRect.xMax - CellPadding + cellRightOffset - ScrollSize - IconSize, cellRect.y + CellPadding, IconSize, IconSize);
                if (Widgets.ButtonText(btnRect, TranslationCache.FittingBtnAddToSlot.Text)) AddWorn(apparel);

                TooltipHandler.TipRegion(btnRect, TranslationCache.FittingBtnAddToSlot.Tooltip);

                // Body part name
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleLeft;
                var labelRect = new Rect(
                    cellRect.x + IconSize + CellPadding * 2,
                    cellRect.y + CellPadding,
                    cellRect.width - CellPadding * 3 - IconSize * 2,
                    cellRect.height - CellPadding * 2
                );
                Widgets.Label(labelRect, apparel.Def.label);
                TooltipHandler.TipRegion(cellRect, apparel.DefaultTooltip);
            }

            cellRect.y += cellRect.height + CellPadding;
        }

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.EndScrollView();
    }

    private void OnRemoveAllClick(BodyPartGroupDef bodyPartGroupDef)
    {
        _worn.RemoveIf(apparel => apparel.Def.apparel.bodyPartGroups.Contains(bodyPartGroupDef));
        DoSomethingChanged();
    }

    private void AddWorn(ThingContainerApparel apparel)
    {
        _worn.RemoveIf(containerApparel => !ApparelUtility.CanWearTogether(containerApparel.Def, apparel.Def, BodyDefOf.Human));
        _worn.Add(apparel);
        _selectedBodyPart = null;
        DoSomethingChanged();
    }
}