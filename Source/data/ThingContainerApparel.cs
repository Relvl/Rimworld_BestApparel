using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public class ThingContainerApparel : AThingContainer
    {
        public static readonly List<ThingContainerApparel> AllApparels = new List<ThingContainerApparel>();
        public static readonly List<ApparelLayerDef> Layers = new List<ApparelLayerDef>();
        public static readonly List<BodyPartGroupDef> BodyParts = new List<BodyPartGroupDef>();

        public static readonly List<AStatProcessor> StatProcessors = new List<AStatProcessor>();
        public static readonly List<ThingCategoryDef> Categories = new List<ThingCategoryDef>();
        public static readonly List<StuffCategoryDef> Stuffs = new List<StuffCategoryDef>();

        private ThingContainerApparel(ThingDef thingDef) : base(thingDef)
        {
        }

        public static void ClearThingDefs()
        {
            AllApparels.Clear();
            Layers.Clear();
            Stuffs.Clear();
            BodyParts.Clear();
            Categories.Clear();
            StatProcessors.Clear();
        }

        public static void TryToAddThingDef(ThingDef thingDef)
        {
            if (thingDef.IsApparel)
            {
                try
                {
                    if (Prefs.DevMode && BestApparel.Config.DoLogThingsLoading)
                    {
                        Log.Message($"--> BestApparel process thingDef: {thingDef.defName}");
                    }

                    AllApparels.Add(new ThingContainerApparel(thingDef));
                }
                catch (Exception e)
                {
                    Log.Warning($"Can not make apparel cache for ThingDef '{thingDef.defName}' -> NRE {e.Message}");
                }
            }
        }

        public static void FinalyzeThingDefs()
        {
            Layers.AddRange(
                AllApparels //
                    .SelectMany(it => it.Def.apparel.layers)
                    .Where(it => it != null)
                    .Distinct()
            );

            Stuffs.AddRange(
                AllApparels //
                    .Where(it => it.Def.stuffProps?.categories != null)
                    .SelectMany(it => it.Def.stuffProps?.categories)
                    .Distinct()
            );

            BodyParts.AddRange(
                AllApparels //
                    .SelectMany(it => it.Def.apparel.bodyPartGroups)
                    .Where(it => it != null)
                    .Distinct()
            );

            Categories.AddRange(
                AllApparels //
                    .Where(it => it.Def.thingCategories != null)
                    .SelectMany(it => it.Def.thingCategories)
                    .GroupBy(it => it.defName)
                    .Select(it => it.First())
            );


            var tmpStatProcessors = new List<AStatProcessor>();

            // Base stats
            tmpStatProcessors.AddRange(
                AllApparels.SelectMany(
                    apparel => DefDatabase<StatDef>.AllDefs //
                        .Where(def => def.Worker.ShouldShowFor(StatRequest.For(apparel.DefaultThing)) && !def.Worker.IsDisabledFor(apparel.DefaultThing))
                        .Select(def => new BaseStatProcessor(def))
                        .Where(processor => !processor.IsValueDefault(apparel.DefaultThing))
                )
            );

            // Equipped stats
            tmpStatProcessors.AddRange(
                AllApparels //
                    .Where(apparel => apparel.Def.equippedStatOffsets != null)
                    .SelectMany(
                        apparel => apparel.Def.equippedStatOffsets //
                            .Where(modifier => modifier != null)
                            .Select(statModifier => new CommonStatProcessor(statModifier.stat))
                            .Where(processor => !processor.IsValueDefault(apparel.DefaultThing))
                    )
            );

            StatProcessors.AddRange(
                tmpStatProcessors //
                    .GroupBy(it => it.GetDefName())
                    .Select(it => it.First())
                    .OrderBy(it => it.GetDefLabel())
            );
        }

        public override bool CheckForFilters()
        {
            if (!BestApparel.Config.ApparelLayers.IsCollectionAllowed(Def.apparel.layers)) return false;
            if (!BestApparel.Config.ApparelBodyParts.IsCollectionAllowed(Def.apparel.bodyPartGroups)) return false;
            if (!BestApparel.Config.ApparelCategories.IsCollectionAllowed(Def.thingCategories)) return false;
            return true;
        }

        public override void MakeCache()
        {
            CachedCells = BestApparel.Config.Columns.Apparel //
                .Select(
                    defName =>
                    {
                        var proc = StatProcessors.FirstOrDefault(it => it.GetDefName() == defName);
                        return proc == null ? null : new CellData(proc, DefaultThing, BestApparel.Config.Sorting.Apparel[proc.GetDefName()] + Config.MaxSortingWeight);
                    }
                )
                .Where(it => it != null)
                .OrderByDescending(c => c.WeightFactor)
                .ThenBy(c => c.DefLabel)
                .ToArray();
        }

        public override void MakeSortingWeightsCache()
        {
            foreach (var defName in BestApparel.Config.Columns.Apparel)
            {
                var thisCell = CachedCells.FirstOrDefault(c => c.DefName == defName);
                if (thisCell == null) continue;

                var rawValues = DataProcessor.CachedApparels //
                    .Select(a => a.CachedCells.FirstOrDefault(c => c.DefName == defName))
                    .Where(v => v != null)
                    .Select(c => c.ValueRaw)
                    .ToArray();
                var valueMin = rawValues.Min();
                var valueMax = rawValues.Max();

                var value = thisCell.ValueRaw;

                thisCell.NormalizedWeight = (value - valueMin) / (valueMax - valueMin);

                if (float.IsNaN(thisCell.NormalizedWeight)) thisCell.NormalizedWeight = 0f;

                if (Prefs.DevMode)
                {
                    thisCell.Tooltips.Add($"StatDefName: {thisCell.DefName} (min: {valueMin}, this: {value} ({thisCell.NormalizedWeight}), max: {valueMax})");
                }

                thisCell.Tooltips.Add("BestApparel.Label.RangePercent".Translate(Math.Round(thisCell.NormalizedWeight * 100f, 1), thisCell.WeightFactor));
            }

            CachedSortingWeight = CachedCells.Sum(c => c.NormalizedWeight * c.WeightFactor);
        }
    }
}