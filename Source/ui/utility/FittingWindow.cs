using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestApparel.container_factory;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui.utility;

public class FittingWindow : Window, IReloadObserver
{
    public override Vector2 InitialSize => new(650, 500);

    private readonly ApparelTabRenderer _parent;
    private Vector2 _scrollLeft = Vector2.zero;
    private Vector2 _scrollRight = Vector2.zero;

    private readonly List<Apparel> _worn = new();
    private readonly List<ThingContainerApparel> _apparelsFiltered = new();
    private readonly List<string> _pawnInitialWorn = new();
    private float _lastFrameListHeight;
    private string _search = "";
    private BodyPartGroupDef _selectedBodyPart;
    private string _pawnInitialName = "";

    private const int CellPadding = 3;
    private const int BpCellHeight = 20;
    private const int IconSize = 24;
    private const int APCellHeight = IconSize + CellPadding * 2;
    private const float ScrollSize = 16;

    public FittingWindow(ApparelTabRenderer parent)
    {
        resizeable = true;
        draggable = true;
        doCloseX = true;
        _parent = parent;
    }

    public override void PreOpen()
    {
        base.PreOpen();
        _worn.ReplaceWith(
            BestApparel.Config.FittingWorn.Select(
                    w =>
                    {
                        var thingDef = DefDatabase<ThingDef>.GetNamed(w);
                        return thingDef.MadeFromStuff ? ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef)) : ThingMaker.MakeThing(thingDef);
                    }
                )
                .Cast<Apparel>()
                .Where(it => it != null)
        );
        DoSomethingChanged();
        BestApparel.Config.ReloadObservers.Add(this);
    }

    public override void PreClose()
    {
        BestApparel.Config.ReloadObservers.Remove(this);
        Config.ModInstance?.WriteSettings();
    }

    public void OnDataProcessorReloaded(ReloadPhase phase)
    {
        if (phase == ReloadPhase.Sorted)
        {
            DoSomethingChanged();
        }
    }

    private void DoSomethingChanged(bool changed = true)
    {
        if (!changed) return;
        _apparelsFiltered.ReplaceWith(
            _parent.AllContainers.ToList()
                .Cast<ThingContainerApparel>()
                .Where(
                    it =>
                    {
                        if (!it.IsSearchAccept(_search)) return false;
                        if (_selectedBodyPart != null) return it.Def.apparel.bodyPartGroups.Contains(_selectedBodyPart);
                        if (_worn.Any(worn => !ApparelUtility.CanWearTogether(worn.def, it.Def, BodyDefOf.Human))) return false;
                        return true;
                    }
                )
                .OrderByDescending(it => it.CachedSortingWeight)
                .ThenBy(it => it.Def.label)
        );
        BestApparel.Config.FittingWorn.ReplaceWith(_worn.Select(it => it.def.defName));
    }

    public override void DoWindowContents(Rect inRect)
    {
        // window title 
        Text.Font = GameFont.Medium;
        GUI.color = Color.green;
        Widgets.Label(inRect, TranslationCache.FittingWindowTitle.Text);

        // Title info icon
        GUI.color = UIUtils.ColorWhiteA50;
        var infoRect = new Rect(TranslationCache.FittingWindowTitle.Size.x + 8, inRect.y + 8, 16, 16);
        MouseoverSounds.DoRegion(infoRect);
        Widgets.ButtonImage(infoRect, TexButton.Info, GUI.color);
        GUI.DrawTexture(infoRect, TexButton.Info);
        TooltipHandler.TipRegion(infoRect, TranslationCache.FittingWindowTitle.Tooltip);
        UIHighlighter.HighlightOpportunity(infoRect, "InfoCard");

        inRect.yMin += 46;

        // =================================================
        var panesHeight = inRect.height - IconSize - CellPadding;

        var leftPaneRect = new Rect(0, inRect.yMin, Math.Max(300, inRect.width / 2 - 30), panesHeight);
        RenderLeftPane(leftPaneRect);

        var rightPaneRect = new Rect(leftPaneRect.xMax + CellPadding, inRect.yMin, inRect.width - leftPaneRect.width - CellPadding * 2, panesHeight);
        RenderRightPane(rightPaneRect);

        var btnRect = new Rect(0, inRect.yMax - IconSize, 100, IconSize);
        AddBottomButton(
            ref btnRect,
            TranslationCache.FittingBtnClear,
            () =>
            {
                _worn.Clear();
                _pawnInitialWorn.Clear();
                _pawnInitialName = "";
                DoSomethingChanged();
            }
        );

        AddBottomButton(
            ref btnRect,
            TranslationCache.FittingBtnFromPawn,
            () =>
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
                                        _worn.ReplaceWith(pawn.apparel.WornApparel);
                                        _pawnInitialWorn.ReplaceWith(pawn.apparel.WornApparel.Select(it => it.def.defName));
                                        _pawnInitialName = pawn.NameFullColored;
                                        DoSomethingChanged();
                                    }
                                )
                            )
                            .Cast<FloatMenuOption>()
                            .ToList()
                    )
                );
            }
        );

        if (_worn.Any())
        {
            AddBottomButton(
                ref btnRect,
                TranslationCache.FittingBtnPin,
                () =>
                {
                    var title = TranslationCache.FittingLetterTotals.Text;
                    if (_pawnInitialName != "") title += $" ({_pawnInitialName})";
                    var sb = new StringBuilder();

                    var tipAdded = false;
                    if (_pawnInitialWorn.Count > 0)
                    {
                        _pawnInitialWorn.ForEach(
                            w =>
                            {
                                if (_worn.Any(apparel => apparel.def.defName == w)) return;
                                var apparelDef = DefDatabase<ThingDef>.GetNamed(w);
                                if (apparelDef is null) return;
                                if (!tipAdded)
                                {
                                    tipAdded = true;
                                    sb.AppendLine("\n");
                                    sb.AppendLine(TranslationCache.FittingLetterRemove.Text);
                                }

                                sb.AppendLine("- " + apparelDef.label.Colorize(Color.grey));
                            }
                        );
                        sb.AppendLine("\n");
                    }

                    tipAdded = false;
                    foreach (var added in _worn.Where(w => !_pawnInitialWorn.Contains(w.def.defName)))
                    {
                        if (!tipAdded)
                        {
                            tipAdded = true;
                            sb.AppendLine(TranslationCache.FittingLetterAdd.Text);
                        }

                        sb.AppendLine("+ " + added.def.label.Colorize(Color.green));
                    }

                    Find.LetterStack.ReceiveLetter(title, sb.ToString().TrimEndNewlines(), LetterDefOf.PositiveEvent);
                }
            );
        }

        // =================================================
    }

    private static void AddBottomButton(ref Rect btnRect, TranslationCache.E text, Action action)
    {
        btnRect.width = text.Size.x + 16;
        if (Widgets.ButtonText(btnRect, text.Text)) action();
        if (text.Tooltip.Length > 0) TooltipHandler.TipRegion(btnRect, text.Tooltip);
        btnRect.x += btnRect.width + CellPadding;
    }

    private void RenderLeftPane(Rect paneRect)
    {
        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var cellRightOffset = _lastFrameListHeight > paneRect.height ? ScrollSize : 0;
        var scrollInnerRect = new Rect(0, 0, paneRect.width - CellPadding * 2 - cellRightOffset, _lastFrameListHeight);

        Widgets.BeginScrollView(paneRect, ref _scrollLeft, scrollInnerRect);
        var cellRect = new Rect(CellPadding, CellPadding, scrollInnerRect.width, BpCellHeight);
        foreach (var bodyPart in _parent.BodyParts.ToList())
        {
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            var part = bodyPart;
            var apparels = _worn.Where(it => it.def.apparel?.bodyPartGroups?.Contains(part) ?? false).ToArray();

            cellRect.height = BpCellHeight + (apparels.Length > 0 ? apparels.Length * 26 : 18);

            GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (Mouse.IsOver(cellRect)) GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (_selectedBodyPart == bodyPart) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

            // body part name
            Text.Font = GameFont.Tiny;
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
                GUI.color = UIUtils.ColorWhiteA50;

                for (var thingIdx = 0; thingIdx < apparels.Length; thingIdx++)
                {
                    var apparel = apparels[thingIdx];
                    var offset = 16 + thingIdx * 26;
                    Widgets.ThingIcon(new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding + offset, IconSize, IconSize), apparel);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    var labelRect = new Rect(cellRect.x + IconSize + CellPadding * 2, cellRect.y + CellPadding + offset, scrollInnerRect.width, IconSize);
                    Widgets.Label(labelRect, apparel.def.label);
                    TooltipHandler.TipRegion(
                        labelRect,
                        $"{apparel.LabelNoParenthesisCap.AsTipTitle()}{GenLabel.LabelExtras(apparel, 1, true, true)}\n\n{apparel.DescriptionDetailed}"
                    );

                    var removeRect = new Rect(cellRect.xMax - CellPadding + cellRightOffset - ScrollSize - IconSize, cellRect.y + CellPadding + offset, IconSize, IconSize);
                    GUI.color = Color.red;
                    if (Mouse.IsOver(removeRect)) GUI.color = Color.yellow;
                    if (Widgets.ButtonText(removeRect, "-"))
                    {
                        _worn.RemoveIf(w => w.def.defName == apparel.def.defName);
                        DoSomethingChanged();
                    }

                    TooltipHandler.TipRegion(removeRect, TranslationCache.FittingBtnRemove.Text);
                }
            }
            else
            {
                Text.Font = GameFont.Tiny;
                GUI.color = UIUtils.ColorWhiteA50;
                Widgets.Label(new Rect(cellRect.x + CellPadding, cellRect.y + CellPadding + 16, scrollInnerRect.width, IconSize), TranslationCache.FittingLabelSelectApparel.Text);
            }

            if (Widgets.ButtonInvisible(cellRect))
            {
                _selectedBodyPart = _selectedBodyPart == null || _selectedBodyPart != bodyPart ? bodyPart : null;
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
        paneRect.height -= IconSize + CellPadding;

        Text.Font = GameFont.Tiny;
        GUI.color = UIUtils.ColorWhiteA50;
        var shownType = BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings.Text : TranslationCache.ControlUseCraftableThings.Text;
        if (_selectedBodyPart != null) shownType += " " + "BestApparel.Fitting.Label.Sown.Part".Translate(_selectedBodyPart.label);
        Widgets.Label(paneRect, "BestApparel.Fitting.Label.Sown".Translate(shownType));
        Text.Font = GameFont.Small;
        GUI.color = Color.white;

        paneRect.yMin += IconSize + CellPadding;

        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var apparelsFiltered = _apparelsFiltered.ToList();

        var scrolledHeight = (APCellHeight + CellPadding) * apparelsFiltered.Count + CellPadding * 2;
        var cellRightOffset = scrolledHeight > paneRect.height ? ScrollSize : 0;
        var scrollInnerRect = new Rect(0, 0, paneRect.width - CellPadding * 2 - cellRightOffset, scrolledHeight);

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
                if (Widgets.ButtonText(btnRect, TranslationCache.FittingBtnAddToSlot.Text)) AddWorn(apparel.DefaultThing as Apparel);

                TooltipHandler.TipRegion(btnRect, TranslationCache.FittingBtnAddToSlot.Tooltip);

                if (_selectedBodyPart != null)
                {
                    var somethingBlocking = _worn.Any(w => !ApparelUtility.CanWearTogether(w.def, apparel.Def, BodyDefOf.Human));
                    if (somethingBlocking)
                    {
                        GUI.color = Color.grey;
                        TooltipHandler.TipRegion(cellRect, TranslationCache.FittingTipSlotOccupied.Tooltip);
                    }
                    else
                    {
                        GUI.color = Color.yellow;
                    }
                }

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

        // search box
        var searchRect = new Rect(paneRect.x, paneRect.yMax + CellPadding, IconSize, IconSize);
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
    }

    private void OnRemoveAllClick(BodyPartGroupDef bodyPartGroupDef)
    {
        _worn.RemoveIf(apparel => apparel.def.apparel.bodyPartGroups.Contains(bodyPartGroupDef));
        DoSomethingChanged();
    }

    private void AddWorn(Apparel newApparel)
    {
        _worn.RemoveIf(it => !ApparelUtility.CanWearTogether(it.def, newApparel.def, BodyDefOf.Human));
        _worn.Add(newApparel);
        _selectedBodyPart = null;
        DoSomethingChanged();
    }
}