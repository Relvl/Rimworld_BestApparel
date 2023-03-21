using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.ui.utility;
using CombatExtended;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui;

// ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
public class MainTabWindow : RimWorld.MainTabWindow
{
    private const int CellPadding = 2;
    private const int NameCellWidth = 200;
    private const int CellHeight = 24;
    private const int HeaderHeight = 36;

    private Vector2 _scrollPosition = Vector2.zero;
    private Vector2 _lastFrameTableSize = Vector2.zero;
    private int _lastFrameRow = -1;
    public readonly DataProcessor DataProcessor;

    public MainTabWindow()
    {
        // super
        doCloseX = true;
        closeOnClickedOutside = false;
        DataProcessor = new DataProcessor();
    }

    public override void PreOpen()
    {
        base.PreOpen();
        DataProcessor.OnMainWindowPreOpen();
    }

    public override void PreClose()
    {
        Find.WindowStack.TryRemove(typeof(FilterWindow));
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.TryRemove(typeof(FittingWindow));
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        inRect.yMin += 35f;

        TabDrawer.DrawTabs(
            inRect,
            new List<TabRecord>
            {
                new(TranslationCache.Apparel.Text, () => BestApparel.Config.SelectedTab = TabId.Apparel, BestApparel.Config.SelectedTab == TabId.Apparel),
                new(TranslationCache.Ranged.Text, () => BestApparel.Config.SelectedTab = TabId.Ranged, BestApparel.Config.SelectedTab == TabId.Ranged),
                new(TranslationCache.Melee.Text, () => BestApparel.Config.SelectedTab = TabId.Melee, BestApparel.Config.SelectedTab == TabId.Melee),
            }
        );

        inRect.yMin += 10f;

        UIUtils.DrawButtonsRowLTR(
            ref inRect,
            (TranslationCache.BtnColumns, OnColumnsClick),
            (TranslationCache.BtnFilter, OnFilterClick),
            (TranslationCache.BtnSorting, OnSortingClick)
        );

        UIUtils.DrawButtonsRowRight(
            ref inRect, //
            (TranslationCache.BtnRangedRestoreAmmo, OnRangedRestoreAmmoClick, BestApparel.Config.SelectedTab == TabId.Ranged)
        );

        inRect.yMin += 34;
        UIUtils.DrawLineAtTop(ref inRect, true, 1);

        RenderTabContent(inRect, DataProcessor.GetTable(BestApparel.Config.SelectedTab));

        // Absolute positions here
        const int collectTypeWidth = 150;
        const int btnWidth = 100;

        var collectTypeRect = new Rect(windowRect.width - Margin * 2 - 10 - collectTypeWidth - btnWidth - 10, 8, collectTypeWidth, 24);
        UIUtils.RenderCheckboxLeft(
            ref collectTypeRect,
            (BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings : TranslationCache.ControlUseCraftableThings).Text,
            BestApparel.Config.UseAllThings,
            state =>
            {
                BestApparel.Config.UseAllThings = state;
                DataProcessor.OnMainWindowPreOpen();
            }
        );
        TooltipHandler.TipRegion(collectTypeRect, (BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings : TranslationCache.ControlUseCraftableThings).Tooltip);

        var fittingButtonWidth = TranslationCache.BtnFitting.Size.x + 24;
        var fittingButtonRect = new Rect(inRect.width - fittingButtonWidth - 10, 4, fittingButtonWidth, TranslationCache.BtnFitting.Size.y + 10);
        if (Widgets.ButtonText(fittingButtonRect, TranslationCache.BtnFitting.Text))
        {
            Find.WindowStack.TryRemove(typeof(FittingWindow));
            Find.WindowStack.Add(new FittingWindow(this));
        }

        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
    }

    private void RenderTabContent(Rect inRect, IReadOnlyList<AThingContainer> thingContainers)
    {
        if (!thingContainers.Any()) return;

        // Header
        var firstContainer = thingContainers.First();
        GUI.color = UIUtils.ColorWhiteA50;
        Text.Anchor = TextAnchor.MiddleCenter;
        Text.Font = GameFont.Tiny;
        var headerRect = new Rect(inRect.x + CellHeight * 2 + NameCellWidth + CellPadding * 3 - _scrollPosition.x, inRect.y, 0, HeaderHeight);
        foreach (var cell in firstContainer.CachedCells)
        {
            headerRect.width = cell.Processor.CellWidth;
            Widgets.Label(headerRect, cell.DefLabel);
            TooltipHandler.TipRegion(headerRect, cell.DefLabel);
            headerRect.x += headerRect.width + CellPadding - 2;
            headerRect.width = 1;
            GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
            headerRect.x += headerRect.width;
        }

        inRect.yMin += HeaderHeight;

        headerRect.x = inRect.x;
        headerRect.y = inRect.y;
        headerRect.width = inRect.width - 16;
        headerRect.height = 1;
        GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
        inRect.yMin += headerRect.height;

        // Table

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;

        var innerScrolledRect = new Rect(0, 0, _lastFrameTableSize.x, _lastFrameTableSize.y);

        Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);

        var mouseOverAnyCell = false;

        for (var idx = 0; idx < thingContainers.Count; idx++)
        {
            var container = thingContainers[idx];

            var rowRect = new Rect(0, CellHeight * idx, 0, CellHeight);
            var isInViewRange = _scrollPosition.y < CellHeight * idx + CellHeight * 2 && _scrollPosition.y + inRect.height > CellHeight * idx;
            if (isInViewRange)
            {
                var cellRect = new Rect(0, CellHeight * idx, CellHeight, CellHeight);

                // i
                Widgets.InfoCardButtonCentered(cellRect, container.DefaultThing);

                // Icon
                cellRect.x += CellHeight + CellPadding;
                Widgets.ThingIcon(cellRect, container.DefaultThing);

                // Label
                Text.Font = GameFont.Tiny;
                cellRect.x += CellHeight + CellPadding;
                cellRect.width = NameCellWidth;
                Widgets.Label(cellRect, container.DefaultThing.def.label);
                Text.Font = GameFont.Small;

                if (Prefs.DevMode)
                {
                    TooltipHandler.TipRegion(cellRect, $"Dev: \nDefName: {container.Def.defName}\nTotal sorting weight: {container.CachedSortingWeight}");
                }

                TooltipHandler.TipRegion(cellRect, container.DefaultTooltip);

                // Columns
                cellRect.x += NameCellWidth + CellPadding;

                for (var cellIdx = 0; cellIdx < container.CachedCells.Length; cellIdx++)
                {
                    var cell = container.CachedCells[cellIdx];

                    cellRect.width = cell.Processor.CellWidth;

                    if (_lastFrameRow == cellIdx) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

                    if (Mouse.IsOver(cellRect))
                    {
                        _lastFrameRow = cellIdx;
                        mouseOverAnyCell = true;
                    }

                    cell.Processor.RenderCell(cellRect, cell, this);

                    // offset to the right
                    cellRect.x += /*todo config? auto-calc?*/ cellRect.width + CellPadding - 1;
                }

                // bg and mouseover
                rowRect.width = cellRect.x + cellRect.width;
                if (Mouse.IsOver(rowRect)) GUI.DrawTexture(rowRect, TexUI.HighlightTex);

                if (idx < thingContainers.Count - 1)
                {
                    UIUtils.DrawLineFull(UIUtils.ColorWhiteA20, CellHeight * (idx + 1), inRect.width);
                }
            }

            _lastFrameTableSize = new Vector2(rowRect.width, rowRect.y + rowRect.height + 16);
        }

        if (!mouseOverAnyCell)
        {
            _lastFrameRow = -1;
        }

        Text.Anchor = TextAnchor.UpperLeft;

        Widgets.EndScrollView();
    }

    private void OnFilterClick()
    {
        Find.WindowStack.TryRemove(typeof(FilterWindow));
        Find.WindowStack.Add(new FilterWindow(this));
    }

    private void OnSortingClick()
    {
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.Add(new SortWindow(this));
    }

    private void OnColumnsClick()
    {
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.Add(new ColumnsWindow(this));
    }

    private void OnRangedRestoreAmmoClick()
    {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        foreach (var container in DataProcessor.GetContainers(TabId.Ranged))
        {
            var thing = container.DefaultThing;
            if (!BestApparel.Config.RangedAmmo.ContainsKey(thing.def.defName)) continue;
            BestApparel.Config.RangedAmmo.Remove(thing.def.defName);
            var ammoDefToLoad = thing.def.Verbs?.FirstOrDefault(it => it is VerbPropertiesCE)?.defaultProjectile?.defName;
            if (ammoDefToLoad.NullOrEmpty()) continue;
            var ammoUser = thing.TryGetComp<CompAmmoUser>();
            var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
            if (link is null) continue;
            ammoUser.CurrentAmmo = link.ammo;
        }
    }
}