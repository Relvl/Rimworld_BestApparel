using System.Collections.Generic;
using System.Linq;
using BestApparel.data;
using BestApparel.ui.utility;
using UnityEngine;
using Verse;

namespace BestApparel.ui;

// ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
public class MainTabWindow : RimWorld.MainTabWindow
{
    private const int CellPadding = 2;
    private const int NameCellWidth = 200;
    private const int CellWidth = 70;
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
        Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
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

        UIUtils.DrawButtonsRow(
            ref inRect,
            85,
            24,
            10,
            (TranslationCache.BtnColumns, OnColumnsClick),
            (TranslationCache.BtnFilter, OnFilterClick),
            (TranslationCache.BtnSorting, OnSortingClick)
        );
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
        var firstContainer = thingContainers.FirstOrDefault();
        if (firstContainer != null)
        {
            GUI.color = BestApparel.ColorWhiteA50;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Tiny;
            var headerRect = new Rect(inRect.x + CellHeight * 2 + NameCellWidth + CellPadding * 3 - _scrollPosition.x, inRect.y, CellWidth, HeaderHeight);
            foreach (var cell in firstContainer.CachedCells)
            {
                headerRect.width = CellWidth;
                Widgets.Label(headerRect, cell.DefLabel);
                TooltipHandler.TipRegion(headerRect, cell.DefLabel);
                headerRect.x += CellWidth + CellPadding - 2;
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
        }

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;

        var innerScrolledRect = new Rect(0, 0, _lastFrameTableSize.x, _lastFrameTableSize.y);

        Widgets.BeginScrollView(inRect, ref _scrollPosition, innerScrolledRect);

        var mouseOverAnyCell = false;

        for (var idx = 0; idx < thingContainers.Count; idx++)
        {
            var container = thingContainers[idx];
            var elementWidth = CellHeight * 2 + CellPadding * 3 + NameCellWidth + container.CachedCells.Length * (CellWidth + CellPadding);
            var elementRect = new Rect(0, CellHeight * idx, elementWidth, CellHeight);

            var isInViewRange = _scrollPosition.y < elementRect.y + elementRect.height * 2 && _scrollPosition.y + inRect.height > elementRect.yMax - elementRect.height;
            if (isInViewRange)
            {
                var cellRect = new Rect(elementRect.x, elementRect.y, CellHeight, CellHeight);

                // i
                Widgets.InfoCardButtonCentered(cellRect, container.DefaultThing);

                // back row click = open info window
                if (Prefs.DevMode && Widgets.ButtonInvisible(elementRect))
                {
                    Find.WindowStack.TryRemove(typeof(ThingInfoWindow));
                    Find.WindowStack.Add(new ThingInfoWindow(this, container.DefaultThing));
                }

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
                    TooltipHandler.TipRegion(cellRect, $"Total sorting weight: {container.CachedSortingWeight}");
                }

                TooltipHandler.TipRegion(cellRect, container.DefaultTooltip);

                // Columns
                cellRect.x += NameCellWidth + CellPadding;
                cellRect.width = CellWidth;

                for (var cellIdx = 0; cellIdx < container.CachedCells.Length; cellIdx++)
                {
                    var cell = container.CachedCells[cellIdx];

                    if (_lastFrameRow == cellIdx)
                    {
                        GUI.DrawTexture(cellRect, TexUI.HighlightTex);
                    }

                    if (Mouse.IsOver(cellRect))
                    {
                        _lastFrameRow = cellIdx;
                        mouseOverAnyCell = true;
                    }

                    if (cell.IsEmpty)
                    {
                        GUI.color = BestApparel.ColorWhiteA20;
                        Widgets.Label(cellRect, cell.Value);
                        GUI.color = Color.white;
                    }
                    else
                    {
                        Widgets.Label(cellRect, cell.Value);
                        foreach (var tooltip in cell.Tooltips)
                        {
                            TooltipHandler.TipRegion(cellRect, tooltip);
                        }
                    }

                    // offset to the right
                    cellRect.x += /*todo config? auto-calc?*/ CellWidth + CellPadding - 1;
                }

                // bg and mouseover
                if (Mouse.IsOver(elementRect))
                {
                    GUI.DrawTexture(elementRect, TexUI.HighlightTex);
                }

                if (idx < thingContainers.Count - 1)
                {
                    UIUtils.DrawLineFull(BestApparel.ColorWhiteA20, CellHeight * (idx + 1), inRect.width);
                }
            }

            _lastFrameTableSize = new Vector2(elementWidth, elementRect.y + elementRect.height + 16);
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

    private void OnIgnoredClick()
    {
    }

    private void OnColumnsClick()
    {
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.Add(new ColumnsWindow(this));
    }
}