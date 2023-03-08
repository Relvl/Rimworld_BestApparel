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
            CachedApparels = ThingContainerApparel.AllApparels //
                .Where(ThingContainerApparel.CheckThingForFilters)
                // Sort: Default
                .OrderBy(it => it.DefaultThing.HitPoints)
                .ThenBy(it => it.DefaultThing.Label)
                .ToArray();
            foreach (var apparel in CachedApparels) apparel.MakeCache();
        }
    }
}