using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.data.impl;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data;

public class DataProcessor
{
    private static ReadOnlyDictionary<TabId, T> MakeTabIdDictionary<T>(Func<T> provider) => new(Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, _ => provider()));

    private readonly List<ThingContainerApparel> _allApparels = new();

    private readonly ReadOnlyDictionary<TabId, List<AThingContainer>> _thingContainers = MakeTabIdDictionary(() => new List<AThingContainer>());
    private readonly ReadOnlyDictionary<TabId, List<AStatProcessor>> _statProcessors = MakeTabIdDictionary(() => new List<AStatProcessor>());
    private readonly ReadOnlyDictionary<TabId, List<StuffCategoryDef>> _stuffs = MakeTabIdDictionary(() => new List<StuffCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, List<ThingCategoryDef>> _categories = MakeTabIdDictionary(() => new List<ThingCategoryDef>());

    private readonly List<ApparelLayerDef> _apparelLayers = new();
    private readonly List<BodyPartGroupDef> _apparelBodyParts = new();
    private readonly List<WeaponClassDef> _rangedClasses = new();

    private void Clear()
    {
        _allApparels.Clear();
        foreach (var (_, collection) in _thingContainers) collection.Clear();
        foreach (var (_, collection) in _statProcessors) collection.Clear();
        foreach (var (_, collection) in _stuffs) collection.Clear();
        foreach (var (_, collection) in _categories) collection.Clear();
        _apparelLayers.Clear();
        _apparelBodyParts.Clear();
        _rangedClasses.Clear();
    }

    public void Rebuild()
    {
        Clear();

        var factory = new ContainerFactory();

        var totalThingDefs = BestApparel.Config.UseAllThings
            ? DefDatabase<ThingDef>.AllDefs
            : Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>()
                .SelectMany(it => it.def.AllRecipes)
                .Where(it => it.AvailableNow && it.ProducedThingDef != null)
                .Select(it => it.ProducedThingDef);
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

        var groupedContainers = totalThingContaners.GroupBy(it => it.GetTabId()).ToList();
        var filtered = MakeTabIdDictionary(() => new List<AThingContainer>());
        foreach (var grouping in groupedContainers)
        {
            var tabId = grouping.Key;

            if (tabId == TabId.Apparel) _allApparels.AddRange(grouping.Cast<ThingContainerApparel>());

            _statProcessors[tabId]
                .AddRange(
                    grouping //
                        .SelectMany(it => it.CollectStats())
                        .OrderByDescending(it => it.GetDefLabel())
                        .GroupBy(it => it.GetDefName())
                        .Select(it => it.First())
                );

            // Filter actual rows
            filtered[tabId].AddRange(grouping.Where(it => it.CheckForFilters()));
        }


        foreach (var (tabId, list) in filtered)
        {
            // Collect minmax 
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
            case TabId.Melee:

                break;
        }
    }

    public IEnumerable<(IEnumerable<Def>, TranslationCache.E, FeatureEnableDisable)> GetFilterData(TabId tabId)
    {
        switch (tabId)
        {
            case TabId.Apparel:
                yield return (_apparelLayers, TranslationCache.FilterLayers, BestApparel.Config.Apparel.Layer);
                yield return (_apparelBodyParts, TranslationCache.FilterBodyParts, BestApparel.Config.Apparel.BodyPart);
                yield return (_categories[TabId.Apparel], TranslationCache.FilterCategory, BestApparel.Config.Apparel.Category);
                yield return (_stuffs[TabId.Apparel], TranslationCache.FilterStuff, BestApparel.Config.Apparel.Stuff);
                break;
            case TabId.Ranged:
                yield return (_rangedClasses, TranslationCache.FilterCategory, BestApparel.Config.Ranged.Category);
                yield return (_categories[TabId.Ranged], TranslationCache.FilterWeaponClass, BestApparel.Config.Ranged.WeaponClass);
                yield return (_stuffs[TabId.Apparel], TranslationCache.FilterStuff, BestApparel.Config.Ranged.Stuff);
                break;
        }
    }

    private static void FinalizeDefs<T>(List<T> collection) where T : Def
    {
        var final = collection //
            .GroupBy(it => it.defName)
            .Select(it => it.First())
            .OrderBy(it => it.label)
            .ToList();
        collection.Clear();
        collection.AddRange(final);
    }

    public IReadOnlyList<AThingContainer> GetTable(TabId tabId) => _thingContainers[tabId];

    public IEnumerable<AStatProcessor> GetStatProcessors(TabId tabId) => _statProcessors[tabId];

    public IReadOnlyList<ThingContainerApparel> GetAllApparels() => _allApparels;
    public IReadOnlyList<BodyPartGroupDef> GetApparelBodyParts() => _apparelBodyParts;

    public ThingContainerApparel GetApparelOfDef(Apparel apparel) => _allApparels.FirstOrDefault(a => a.Def.defName == apparel.def.defName);
}