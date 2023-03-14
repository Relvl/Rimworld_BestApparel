using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

    private readonly ReadOnlyDictionary<TabId, HashSet<AThingContainer>> _containers = MakeTabIdDictionary(() => new HashSet<AThingContainer>());
    private readonly ReadOnlyDictionary<TabId, List<AThingContainer>> _filteredContainers = MakeTabIdDictionary(() => new List<AThingContainer>());

    private readonly ReadOnlyDictionary<TabId, List<AStatProcessor>> _stats = MakeTabIdDictionary(() => new List<AStatProcessor>());

    private readonly ReadOnlyDictionary<TabId, HashSet<StuffCategoryDef>> _stuffs = MakeTabIdDictionary(() => new HashSet<StuffCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, HashSet<ThingCategoryDef>> _categories = MakeTabIdDictionary(() => new HashSet<ThingCategoryDef>());
    private readonly ReadOnlyDictionary<TabId, HashSet<WeaponClassDef>> _weaponClasses = MakeTabIdDictionary(() => new HashSet<WeaponClassDef>());

    private readonly HashSet<ApparelLayerDef> _apparelLayers = new();
    private readonly HashSet<BodyPartGroupDef> _apparelBodyParts = new();

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
            foreach (var workTable in Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>())
            {
                foreach (var recipeDef in workTable.def.AllRecipes)
                {
                    var thingDef = recipeDef.ProducedThingDef;
                    if (thingDef is null) continue;
                    if (!recipeDef.AvailableNow) continue; // todo much calculations, check it all
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
        _apparelLayers.Clear();
        _apparelBodyParts.Clear();

        var tmpStats = MakeTabIdDictionary(() => new HashSet<AStatProcessor>());
        var factory = new ContainerFactory();

        // Collect data
        foreach (var container in GetAllAvailableThingDefs().Select(it => factory.Produce(it)).Where(it => it != null))
        {
            _containers[container.GetTabId()].Add(container);

            if (container.Def.thingCategories != null) _categories[container.GetTabId()].AddRange(container.Def.thingCategories);
            if (container.Def.stuffProps?.categories != null) _stuffs[container.GetTabId()].AddRange(container.Def.stuffProps?.categories);
            if (container.Def.apparel?.layers != null) _apparelLayers.AddRange(container.Def.apparel.layers);
            if (container.Def.apparel?.bodyPartGroups != null) _apparelBodyParts.AddRange(container.Def.apparel.bodyPartGroups);
            if (container.Def.weaponClasses != null) _weaponClasses[container.GetTabId()].AddRange(container.Def.weaponClasses);

            foreach (var stat in container.CollectStats())
            {
                if (tmpStats[container.GetTabId()].Any(s => s.GetDefName() == stat.GetDefName())) continue;
                tmpStats[container.GetTabId()].Add(stat);
            }
        }

        foreach (var (tabId, stats) in tmpStats) _stats[tabId].AddRange(stats);

        Rebuild();
    }

    public void Rebuild()
    {
        foreach (var (_, collection) in _filteredContainers) collection.Clear();

        foreach (var (tabId, containers) in _containers)
        {
            var filtered = containers.Where(container => container.CheckForFilters()).ToList();
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
                yield return (_apparelLayers, TranslationCache.FilterLayers, BestApparel.Config.Apparel.Layer);
                yield return (_apparelBodyParts, TranslationCache.FilterBodyParts, BestApparel.Config.Apparel.BodyPart);
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
    public IEnumerable<BodyPartGroupDef> GetApparelBodyParts() => _apparelBodyParts;
    public ThingContainerApparel GetApparelOfDef(Apparel apparel) => GetAllApparels().FirstOrDefault(a => a.Def.defName == apparel.def.defName);
}