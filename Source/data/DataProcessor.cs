using System.Linq;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public static class DataProcessor
    {
        public static ThingContainerApparel[] CachedApparels { get; private set; } = { };

        public static void CollectData()
        {
            ThingContainerApparel.ClearThingDefs();

            if (Config.Instance.UseAllThings)
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

            MakeCache();
        }

        private static void ProcessThing(ThingDef thingDef)
        {
            ThingContainerApparel.TryToAddThingDef(thingDef);
        }

        private static void MakeCache()
        {
            CachedApparels = ThingContainerApparel.AllApparels.Where(ThingContainerApparel.CheckThingForFilters).ToArray();
            foreach (var apparel in CachedApparels) apparel.MakeCache();
            foreach (var apparel in CachedApparels) apparel.MakeSortingWeightsCache();
            CachedApparels = CachedApparels.OrderByDescending(it => it.CachedSortingWeight).ThenBy(it => it.DefaultThing.Label).ToArray();
        }
    }
}