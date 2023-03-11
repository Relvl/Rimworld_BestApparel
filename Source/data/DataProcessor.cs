using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data;

public class DataProcessor
{
    private static ReadOnlyDictionary<TabId, T> MakeTabIdDictionary<T>(Func<T> provider) => new(Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, _ => provider()));

    private readonly ReadOnlyDictionary<TabId, List<AThingContainer>> _thingContainers = MakeTabIdDictionary(() => new List<AThingContainer>());
    private readonly ReadOnlyDictionary<TabId, List<AStatProcessor>> _statProcessors = MakeTabIdDictionary(() => new List<AStatProcessor>());
    private readonly ReadOnlyDictionary<TabId, List<StuffCategoryDef>> _stuffs = MakeTabIdDictionary(() => new List<StuffCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, List<ThingCategoryDef>> _categories = MakeTabIdDictionary(() => new List<ThingCategoryDef>());

    private readonly List<ApparelLayerDef> _apparelLayers = new();
    private readonly List<BodyPartGroupDef> _apparelBodyParts = new();
    private readonly List<WeaponClassDef> _rangedClasses = new();

    private void Clear()
    {
        foreach (var (_, collection) in _thingContainers) collection.Clear();
        foreach (var (_, collection) in _statProcessors) collection.Clear();
        foreach (var (_, collection) in _stuffs) collection.Clear();
        foreach (var (_, collection) in _categories) collection.Clear();
        _apparelLayers.Clear();
        _apparelBodyParts.Clear();
        _rangedClasses.Clear();
        BestApparel.Config.PrefillSorting();
    }

    public void Rebuild()
    {
        Clear();

        var totalThingDefs = BestApparel.Config.UseAllThings
            ? DefDatabase<ThingDef>.AllDefs
            : Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>()
                .SelectMany(it => it.def.AllRecipes)
                .Where(it => it.AvailableNow && it.ProducedThingDef != null)
                .Select(it => it.ProducedThingDef);

        var factory = new ContainerFactory();

        var totalThingContaners = totalThingDefs //
            .GroupBy(it => it.defName)
            .Select(it => it.First())
            .Select(factory.Produce)
            .Where(it => it != null)
            .ToList();

        // Collect defs for filters 
        totalThingContaners.ForEach(FillDefs);
        foreach (var (_, list) in _categories) FinalizeDefs(list);
        foreach (var (_, list) in _stuffs) FinalizeDefs(list);
        FinalizeDefs(_apparelLayers);
        FinalizeDefs(_apparelBodyParts);
        FinalizeDefs(_rangedClasses);

        // Filter actual rows
        var filteredContainers = totalThingContaners //
            .Where(it => it.CheckForFilters())
            .GroupBy(it => it.GetTabId())
            .ToDictionary(it => it.Key, it => it.ToList());

        foreach (var (tabId, list) in filteredContainers)
        {
            // Collect stats 

            _statProcessors[tabId]
                .AddRange( //
                    list //
                        .SelectMany(container => container.CollectStats())
                        .GroupBy(it => it.GetDefName())
                        .Select(it => it.First())
                );
            var columns = BestApparel.Config.GetColumnsFor(tabId);
            var columnsProcessors = _statProcessors[tabId].Where(proc => columns.Contains(proc.GetDefName()));

            var columnMinMax = columnsProcessors //
                // Processor to list of actual filtered containers' values
                .ToDictionary( //
                    it => it,
                    it => list.Select(c => it.GetStatValue(c.DefaultThing))
                )
                // Processor to (min, max) values
                .ToDictionary( //
                    it => it.Key,
                    it => (it.Value.Min(), it.Value.Max())
                );

            foreach (var container in list)
            {
                container.CacheCells(columnMinMax);
            }

            _thingContainers[tabId].AddRange(list.OrderByDescending(l => l.CachedSortingWeight).ThenBy(l => l.Def.label));
        }

        Config.ModInstance.WriteSettings();
    }

    private void FillDefs(AThingContainer container)
    {
        if (container.Def.thingCategories != null) _categories[container.GetTabId()].AddRange(container.Def.thingCategories);
        if (container.Def.stuffProps?.categories != null) _stuffs[container.GetTabId()].AddRange(container.Def.stuffProps.categories);

        switch (container.GetTabId())
        {
            case TabId.Apparel:
                if (container.Def.apparel.layers != null) _apparelLayers.AddRange(container.Def.apparel.layers);
                if (container.Def.apparel.bodyPartGroups != null) _apparelBodyParts.AddRange(container.Def.apparel.bodyPartGroups);
                break;
            case TabId.Ranged:
                if (container.Def.weaponClasses != null) _rangedClasses.AddRange(container.Def.weaponClasses);
                break;
        }
    }

    public IEnumerable<(IEnumerable<Def>, string, FeatureEnableDisable)> GetFilterData(TabId tabId)
    {
        switch (tabId)
        {
            case TabId.Apparel:
                yield return (_apparelLayers, "BestApparel.FilterType.Layer", BestApparel.Config.Apparel.Layer);
                yield return (_apparelBodyParts, "BestApparel.FilterType.BodyPart", BestApparel.Config.Apparel.BodyPart);
                yield return (_categories[TabId.Apparel], "BestApparel.FilterType.Category", BestApparel.Config.Apparel.Category);
                yield return (_stuffs[TabId.Apparel], "BestApparel.FilterType.Stuff", BestApparel.Config.Apparel.Stuff);
                break;
            case TabId.Ranged:
                yield return (_rangedClasses, "BestApparel.FilterType.Category", BestApparel.Config.Ranged.Category);
                yield return (_categories[TabId.Ranged], "BestApparel.FilterType.WeaponClass", BestApparel.Config.Ranged.WeaponClass);
                yield return (_stuffs[TabId.Apparel], "BestApparel.FilterType.Stuff", BestApparel.Config.Ranged.Stuff);
                break;
        }
    }

    private static void FinalizeDefs<T>(List<T> collection) where T : Def
    {
        var final = collection.GroupBy(it => it.defName).Select(it => it.First()).ToList();
        collection.Clear();
        collection.AddRange(final);
    }

    public IReadOnlyList<AThingContainer> GetTable(TabId tabId) => _thingContainers[tabId];

    public IEnumerable<AStatProcessor> GetStatProcessors(TabId tabId) => _statProcessors[tabId];
}