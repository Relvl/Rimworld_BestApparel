using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public static class DataProcessor
    {
        public static ThingContainerApparel[] CachedApparels { get; private set; } = { };
        public static ThingContainerRanged[] CachedRanged { get; private set; } = { };

        public static void CollectData()
        {
            BestApparel.Config.PrefillSorting();

            ThingContainerApparel.ClearThingDefs();
            ThingContainerRanged.ClearThingDefs();

            if (BestApparel.Config.UseAllThings)
            {
                DefDatabase<ThingDef>.AllDefs.Distinct().ToList().ForEach(ProcessThing);
            }
            else
            {
                Find.CurrentMap.listerBuildings.allBuildingsColonist.OfType<Building_WorkTable>()
                    .SelectMany(it => it.def.AllRecipes)
                    .Where(it => it.AvailableNow && it.ProducedThingDef != null)
                    .Select(it => it.ProducedThingDef)
                    .Distinct()
                    .ToList()
                    .ForEach(ProcessThing);
            }

            ThingContainerApparel.FinalyzeThingDefs();
            ThingContainerRanged.FinalyzeThingDefs();

            // === Make the Cache

            CachedApparels = ThingContainerApparel.AllApparels.Where(it => it.CheckForFilters()).ToArray();
            foreach (var apparel in CachedApparels) apparel.MakeCache();
            foreach (var apparel in CachedApparels) apparel.MakeSortingWeightsCache();
            CachedApparels = CachedApparels //
                .OrderByDescending(it => it.CachedSortingWeight)
                .ThenBy(it => it.DefaultThing.Label)
                .ToArray();

            CachedRanged = ThingContainerRanged.AllRanged.Where(it => it.CheckForFilters()).ToArray();
            foreach (var ranged in CachedRanged) ranged.MakeCache();
            foreach (var ranged in CachedRanged) ranged.MakeSortingWeightsCache();
            CachedRanged = CachedRanged //
                .OrderByDescending(it => it.CachedSortingWeight)
                .ThenBy(it => it.DefaultThing.Label)
                .ToArray();

            Config.ModInstance.WriteSettings();
        }

        private static void ProcessThing(ThingDef thingDef)
        {
            ThingContainerApparel.TryToAddThingDef(thingDef);
            ThingContainerRanged.TryToAddThingDef(thingDef);
        }
    }
}