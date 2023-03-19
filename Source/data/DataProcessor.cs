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
    public readonly IReadOnlyList<BodyPartGroupDef> ApparelBodyParts = DefDatabase<ThingDef>.AllDefs //
        .Where(d => d.IsApparel && d.apparel.bodyPartGroups != null)
        .SelectMany(d => d.apparel.bodyPartGroups)
        .GroupBy(d => d.defName)
        .Select(d => d.First())
        .ToList();

    public readonly IReadOnlyList<ApparelLayerDef> ApparelLayers = DefDatabase<ThingDef>.AllDefs //
        .Where(d => d.IsApparel && d.apparel.layers != null)
        .SelectMany(d => d.apparel.layers)
        .GroupBy(d => d.defName)
        .Select(d => d.First())
        .ToList();

    private static ReadOnlyDictionary<TabId, T> MakeTabIdDictionary<T>(Func<T> provider) => new(Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, _ => provider()));

    private readonly ReadOnlyDictionary<TabId, HashSet<AThingContainer>> _containers = MakeTabIdDictionary(() => new HashSet<AThingContainer>());
    private readonly ReadOnlyDictionary<TabId, List<AThingContainer>> _filteredContainers = MakeTabIdDictionary(() => new List<AThingContainer>());

    private readonly ReadOnlyDictionary<TabId, List<AStatProcessor>> _stats = MakeTabIdDictionary(() => new List<AStatProcessor>());

    private readonly ReadOnlyDictionary<TabId, HashSet<StuffCategoryDef>> _stuffs = MakeTabIdDictionary(() => new HashSet<StuffCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, HashSet<ThingCategoryDef>> _categories = MakeTabIdDictionary(() => new HashSet<ThingCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, HashSet<WeaponClassDef>> _weaponClasses = MakeTabIdDictionary(() => new HashSet<WeaponClassDef>());

    public List<IReloadObserver> ReloadObservers { get; } = new();

    private static IEnumerable<ThingDef> GetAllAvailableThingDefs()
    {
        if (BestApparel.Config.UseAllThings)
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                yield return thingDef;
            }
        }
        else
        {
            var returnedThingDefs = new List<string>();
            foreach (var workTable in Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>())
            {
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
    }

    public void OnMainWindowPreOpen()
    {
        // Clear pre-build
        foreach (var (_, collection) in _containers) collection.Clear();
        foreach (var (_, collection) in _stats) collection.Clear();
        foreach (var (_, collection) in _stuffs) collection.Clear();
        foreach (var (_, collection) in _categories) collection.Clear();
        foreach (var (_, collection) in _weaponClasses) collection.Clear();

        var tmpStats = MakeTabIdDictionary(() => new HashSet<AStatProcessor>());
        var factory = new ContainerFactory();

        // Collect data
        foreach (var container in GetAllAvailableThingDefs().Select(it => factory.Produce(it)).Where(it => it != null))
        {
            _containers[container.GetTabId()].Add(container);

            // todo! это на самом деле тоже надо из DefDatabase брать целиком...
            if (container.Def.thingCategories != null) _categories[container.GetTabId()].AddRange(container.Def.thingCategories);
            if (container.Def.stuffProps?.categories != null) _stuffs[container.GetTabId()].AddRange(container.Def.stuffProps?.categories);
            if (container.Def.weaponClasses != null) _weaponClasses[container.GetTabId()].AddRange(container.Def.weaponClasses);

            foreach (var stat in container.CollectStats())
            {
                if (tmpStats[container.GetTabId()].Any(s => s.GetDefName() == stat.GetDefName())) continue;
                tmpStats[container.GetTabId()].Add(stat);
            }
        }

        foreach (var (tabId, stats) in tmpStats) _stats[tabId].AddRange(stats.OrderBy(s => s.GetDefLabel()));

        Rebuild();
    }

    public void Rebuild()
    {
        foreach (var (_, collection) in _filteredContainers) collection.Clear();

        foreach (var (tabId, containers) in _containers)
        {
            if (containers == null) continue;
            var filtered = containers?.Where(container => container?.CheckForFilters() ?? false).ToList();
            var columns = BestApparel.Config.GetColumnsFor(tabId);

            var statMinmax = new Dictionary<AStatProcessor, ( /*min*/float, /*max*/float)>();
            foreach (var stat in _stats[tabId].Where(s => columns.Contains(s.GetDefName())))
            {
                var minmax = statMinmax.ContainsKey(stat) ? statMinmax[stat] : (0, 0);

                foreach (var statValue in filtered.Select(container => stat.GetStatValue(container.DefaultThing)))
                {
                    if (statValue > minmax.Item2) minmax.Item2 = statValue;
                    if (statValue < minmax.Item1) minmax.Item1 = statValue;
                }

                statMinmax[stat] = minmax;
            }

            filtered.ForEach(container => container.CacheCells(statMinmax));

            _filteredContainers[tabId]
                .AddRange(
                    filtered //
                        .OrderByDescending(l => l.CachedSortingWeight)
                        .ThenBy(container => container.Def.label)
                );
        }

        ReloadObservers.ForEach(it => it.OnDataProcessorReloaded());

        Config.ModInstance.WriteSettings();
    }

    public IEnumerable<(IEnumerable<Def>, TranslationCache.E, FeatureEnableDisable)> GetFilterData(TabId tabId)
    {
        switch (tabId)
        {
            case TabId.Apparel:
                yield return (ApparelLayers, TranslationCache.FilterLayers, BestApparel.Config.Apparel.Layer);
                yield return (ApparelBodyParts, TranslationCache.FilterBodyParts, BestApparel.Config.Apparel.BodyPart);
                yield return (_categories[TabId.Apparel], TranslationCache.FilterCategory, BestApparel.Config.Apparel.Category);
                yield return (_stuffs[TabId.Apparel], TranslationCache.FilterStuff, BestApparel.Config.Apparel.Stuff);
                break;
            case TabId.Ranged:
                yield return (_weaponClasses[TabId.Ranged], TranslationCache.FilterCategory, BestApparel.Config.Ranged.Category);
                yield return (_categories[TabId.Ranged], TranslationCache.FilterWeaponClass, BestApparel.Config.Ranged.WeaponClass);
                yield return (_stuffs[TabId.Apparel], TranslationCache.FilterStuff, BestApparel.Config.Ranged.Stuff);
                break;
            case TabId.Melee:
                yield return (_weaponClasses[TabId.Ranged], TranslationCache.FilterCategory, BestApparel.Config.Ranged.Category);
                yield return (_categories[TabId.Ranged], TranslationCache.FilterWeaponClass, BestApparel.Config.Ranged.WeaponClass);
                yield return (_stuffs[TabId.Apparel], TranslationCache.FilterStuff, BestApparel.Config.Ranged.Stuff);
                break;
        }
    }

    public IReadOnlyList<AThingContainer> GetTable(TabId tabId) => _filteredContainers[tabId];
    public IEnumerable<AStatProcessor> GetStatProcessors(TabId tabId) => _stats[tabId];
    public IEnumerable<ThingContainerApparel> GetAllApparels() => _containers[TabId.Apparel].Cast<ThingContainerApparel>();
}