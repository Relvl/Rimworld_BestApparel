using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.def;
using BestApparel.ui;
using RimWorld;
using UnityEngine;
using Verse;

// ReSharper disable MemberCanBeProtected.Global, MemberCanBePrivate.Global

namespace BestApparel.thing_tab_renderer;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by reflection: ThingTabDef.renderClass -> ThingTab:ctor
public class DefaultThnigTabRenderer : IThingTabRenderer
{
    public List<IReloadObserver> ReloadObservers { get; } = new();

    protected const int CellPadding = 2;
    protected const int NameCellWidth = 200;
    protected const int CellHeight = 24;
    protected const int HeaderHeight = 36;

    protected readonly string TabId;
    protected readonly IContainerFactory Factory;
    protected readonly List<IStatCollector> Collectors;
    protected readonly List<AThingContainer> FilteredContainers = new();
    protected readonly List<IContainerPostprocess> Postprocessors = new();

    protected Vector2 Scroll = Vector2.zero;
    protected Vector2 LastFrameTableSize = Vector2.zero;
    protected int LastFrameHighlightRow = -1;

    public readonly HashSet<AThingContainer> AllContainers = new();
    protected readonly HashSet<ThingCategoryDef> Categories = new();
    protected readonly HashSet<StuffCategoryDef> Stuffs = new();
    protected readonly HashSet<WeaponClassDef> WeaponClasses = new();
    protected readonly HashSet<AStatProcessor> StatProcessors = new();

    public DefaultThnigTabRenderer(ThingTabDef def)
    {
        TabId = def.defName;
        Factory = Activator.CreateInstance(def.factoryClass) as IContainerFactory;
        Collectors = def.collectors.Select(c => Activator.CreateInstance(c) as IStatCollector).ToList();

        foreach (var postprocessDef in DefDatabase<ContainerPostprocessDef>.AllDefs.OrderBy(d => d.Order))
        {
            if (!postprocessDef.Tabs.Contains(def.defName)) continue;
            var postprocessor = Activator.CreateInstance(postprocessDef.Postprocessor) as IContainerPostprocess;
            Postprocessors.Add(postprocessor);
        }
    }

    public string GetTabId() => TabId;

    public virtual void DoWindowContents(ref Rect inRect, string searchString)
    {
        var containers = FilteredContainers //
            .Where(c => searchString == "" || c.Def.defName.ToLower().Contains(searchString) || c.Def.label.ToLower().Contains(searchString))
            .ToList(); // copy containers to prevent concurrent modification exceptions
        if (!containers.Any()) return;

        RenderTableHeader(ref inRect, containers.First());
        RenderTable(ref inRect, containers);
    }

    protected virtual void RenderTableHeader(ref Rect inRect, AThingContainer firstContainer)
    {
        Text.Anchor = TextAnchor.MiddleCenter;
        Text.Font = GameFont.Tiny;

        // Header cells
        var headerRect = new Rect(inRect.x + CellHeight * 2 + NameCellWidth + CellPadding * 4 - Scroll.x, inRect.y, 0, HeaderHeight);
        foreach (var cell in firstContainer.CachedCells) RenderHeaderCell(ref headerRect, cell);
        inRect.yMin += HeaderHeight;

        GUI.color = UIUtils.ColorWhiteA50;

        // Final line todo! move to upper level?
        headerRect.x = inRect.x;
        headerRect.y = inRect.y;
        headerRect.width = inRect.width - 16;
        headerRect.height = 1;
        GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
        inRect.yMin += headerRect.height;

        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
    }

    protected virtual void RenderHeaderCell(ref Rect headerRect, CellData cell)
    {
        GUI.color = UIUtils.ColorWhiteA50;
        headerRect.width = cell.Processor.CellWidth;

        if (BestApparel.Config.UseSimpleDataSorting)
        {
            var isSortedColumn = BestApparel.Config.SimpleSorting.ContainsKey(TabId) && BestApparel.Config.SimpleSorting[TabId].First == cell.Processor.GetDefName();
            if (isSortedColumn)
            {
                var order = BestApparel.Config.SimpleSorting[TabId].Second;
                if (order < 0)
                {
                    GUI.color = UIUtils.ColorSortPositive;
                    // todo! arrow!
                }
                else if (order > 0)
                {
                    GUI.color = UIUtils.ColorSortNegative;
                    // todo! arrow!
                }
            }

            if (Mouse.IsOver(headerRect))
            {
                GUI.color = UIUtils.ColorLinkHover;
                if (Widgets.ButtonInvisible(headerRect))
                {
                    var order = 1;
                    if (isSortedColumn)
                    {
                        order = BestApparel.Config.SimpleSorting[TabId].Second * -1;
                    }

                    BestApparel.Config.SimpleSorting[TabId] = new Pair<string, int>(cell.Processor.GetDefName() ?? "--unk--", order);
                    UpdateSort();
                }
            }
        }

        Widgets.Label(headerRect, cell.Processor.GetDefLabel());
        var tooltip = $"{cell.Processor.GetDefLabel().CapitalizeFirst().Colorize(Color.green)}\n\n{cell.Processor.StatDef.description ?? "-no-description-"}";

        if (Prefs.DevMode)
        {
            tooltip += cell.Processor.DebugTooltip();
        }

        TooltipHandler.TipRegion(headerRect, tooltip);
        headerRect.x += headerRect.width + CellPadding - 2;
        headerRect.width = 1;
        GUI.DrawTexture(headerRect, BaseContent.WhiteTex);
        headerRect.x += headerRect.width;
    }

    protected virtual void RenderTable(ref Rect inRect, List<AThingContainer> containers)
    {
        Text.Anchor = TextAnchor.MiddleLeft;

        var innerScrolledRect = new Rect(0, 0, LastFrameTableSize.x, LastFrameTableSize.y);
        Widgets.BeginScrollView(inRect, ref Scroll, innerScrolledRect);

        var mouseOverAnyCell = false;
        for (var idx = 0; idx < containers.Count; idx++)
        {
            var container = containers[idx];
            var rowRect = new Rect(0, CellHeight * idx, 0, CellHeight);
            var isInViewRange = IsRowInViewport(ref inRect, idx);
            if (isInViewRange)
            {
                var cellRect = new Rect(0, CellHeight * idx, CellHeight, CellHeight);

                RenderRowThingLabel(ref cellRect, container);

                for (var cellIdx = 0; cellIdx < container.CachedCells.Length; cellIdx++)
                {
                    var cell = container.CachedCells[cellIdx];
                    cellRect.width = cell.Processor.CellWidth;

                    if (LastFrameHighlightRow == cellIdx) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

                    if (Mouse.IsOver(cellRect))
                    {
                        LastFrameHighlightRow = cellIdx;
                        mouseOverAnyCell = true;
                    }

                    Text.Anchor = TextAnchor.MiddleRight;
                    GUI.color = Color.white;
                    cell.Processor.RenderCell(cellRect, cell, this); // todo! move to this' method

                    // offset to the right
                    cellRect.x += /*todo config? auto-calc?*/ cellRect.width + CellPadding - 1;
                }

                // bg and mouseover
                rowRect.width = cellRect.x + cellRect.width;
                if (Mouse.IsOver(rowRect)) GUI.DrawTexture(rowRect, TexUI.HighlightTex);

                if (idx < containers.Count - 1) UIUtils.DrawLineFull(UIUtils.ColorWhiteA20, CellHeight * (idx + 1), inRect.width);
            }

            LastFrameTableSize = new Vector2(rowRect.width, rowRect.y + rowRect.height + 16);
        }

        if (!mouseOverAnyCell) LastFrameHighlightRow = -1;

        Widgets.EndScrollView();

        Text.Anchor = TextAnchor.UpperLeft;
    }

    protected virtual void RenderRowThingLabel(ref Rect cellRect, AThingContainer container)
    {
        Text.Anchor = TextAnchor.MiddleLeft;

        // i
        cellRect.width = CellHeight;
        Widgets.InfoCardButtonCentered(cellRect, container.DefaultThing);
        cellRect.x += CellHeight + CellPadding;

        // [icon]
        cellRect.width = CellHeight;
        Widgets.ThingIcon(cellRect, container.DefaultThing);
        cellRect.x += CellHeight + CellPadding * 2;

        // [label]
        Text.Font = GameFont.Tiny;

        cellRect.width = NameCellWidth;
        Widgets.Label(cellRect, container.DefaultThing.def.label);

        Text.Font = GameFont.Small;

        if (Prefs.DevMode) TooltipHandler.TipRegion(cellRect, $"Dev: \nDefName: {container.Def.defName}\nTotal sorting weight: {container.CachedSortingWeight}");
        TooltipHandler.TipRegion(cellRect, container.DefaultTooltip);
        cellRect.x += NameCellWidth + CellPadding;
    }

    protected virtual bool IsRowInViewport(ref Rect inRect, int idx) => Scroll.y < CellHeight * idx + CellHeight * 2 && Scroll.y + inRect.height > CellHeight * idx;

    protected virtual IEnumerable<ThingDef> GetAllThingDefs()
    {
        if (BestApparel.Config.UseAllThings)
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
                yield return thingDef;
        }
        else
        {
            var returnedThingDefs = new List<string>();
            foreach (var workTable in Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>())
            foreach (var recipeDef in workTable.def.AllRecipes)
            {
                var thingDef = recipeDef.ProducedThingDef;
                if (thingDef is null) continue;
                if (!recipeDef.AvailableNow) continue; // todo much calculations, check it all
                if (returnedThingDefs.Contains(thingDef.defName)) continue;
                returnedThingDefs.Add(thingDef.defName);
                yield return thingDef;
            }
        }
    }

    public virtual void PrepareCriteria()
    {
        foreach (var def in DefDatabase<ThingDef>.AllDefs)
        {
            if (!Factory.CanProduce(def)) continue;
            PrepareCriteriaEach(def);
        }
    }

    protected virtual void PrepareCriteriaEach(ThingDef def)
    {
        if (def.thingCategories != null) Categories.AddRange(def.thingCategories);
        if (def.stuffProps?.categories != null) Stuffs.AddRange(def.stuffProps.categories);
        if (def.weaponClasses != null) WeaponClasses.AddRange(def.weaponClasses);
    }

    public virtual void CollectContainers()
    {
        DisposeContainers();

        PrepareCriteria(); // todo! на самом деле надо бы это кешировать на всю сессию

        var tmpStatProcessors = new HashSet<AStatProcessor>();
        var allContainers = GetAllThingDefs().Select(def => !Factory.CanProduce(def) ? null : Factory.Produce(def, GetTabId())).Where(con => con is not null).ToList();

        foreach (var collector in Collectors)
        {
            foreach (var container in allContainers)
            {
                collector.Prepare(container.DefaultThing);
            }
        }

        foreach (var container in allContainers)
        {
            PostProcessContainer(container);
            AllContainers.Add(container);
            // only collect stats from available things - just for propper (min,max) value calculation
            // todo! move up to PrepareCriteria - we have column selection
            foreach (var collector in Collectors)
            {
                foreach (var processor in collector.Collect(container.DefaultThing))
                {
                    if (tmpStatProcessors.Any(p => p.GetDefName() == processor.GetDefName())) continue;
                    if (processor.IsValueDefault(container.DefaultThing)) continue;
                    tmpStatProcessors.Add(processor);
                }
            }
        }

        StatProcessors.AddRange(tmpStatProcessors.OrderBy(p => p.GetDefLabel()));

        UpdateFilter();
    }

    public virtual void PostProcessContainer(AThingContainer container)
    {
        foreach (var poroc in Postprocessors) poroc.Postprocess(container, this);
    }

    public virtual void UpdateFilter()
    {
        FilteredContainers.ReplaceWith(AllContainers.Where(container => container.CheckForFilters()));
        UpdateSort();
    }

    public virtual void UpdateSort()
    {
        var filtered = FilteredContainers.ToList();

        var columns = BestApparel.Config.GetColumns(GetTabId());
        var statMinmax = new Dictionary<AStatProcessor, ( /*min*/float, /*max*/float)>();
        foreach (var processor in StatProcessors.Where(s => columns.Contains(s.GetDefName())))
        {
            var minmax = statMinmax.ContainsKey(processor) ? statMinmax[processor] : (0, 0);
            foreach (var statValue in filtered.Select(container => processor.GetStatValue(container.DefaultThing)))
            {
                if (statValue > minmax.Item2) minmax.Item2 = statValue;
                if (statValue < minmax.Item1) minmax.Item1 = statValue;
            }

            statMinmax[processor] = minmax;
        }

        filtered.ForEach(container => container.CacheCells(statMinmax, this));

        FilteredContainers.ReplaceWith(
            filtered //
                .OrderByDescending(c => c.CachedSortingWeight)
                .ThenBy(c => c.Def.label)
        );

        foreach (var observer in ReloadObservers) observer.OnDataProcessorReloaded();
    }

    public virtual void DisposeContainers()
    {
        FilteredContainers.Clear();
        AllContainers.Clear();
        StatProcessors.Clear();
        Categories.Clear();
        Stuffs.Clear();
        WeaponClasses.Clear();
    }

    public virtual IEnumerable<(IEnumerable<Def>, TranslationCache.E, string)> GetFilterData()
    {
        yield return (Categories, TranslationCache.FilterCategory, nameof(ThingCategoryDef));

        if (WeaponClasses.Count > 0)
            yield return (WeaponClasses, TranslationCache.FilterWeaponClass, nameof(WeaponClassDef));
    }

    public virtual IEnumerable<AStatProcessor> GetColumnData() => StatProcessors;

    public virtual HashSet<AThingContainer> GetAllContainers() => AllContainers;
}