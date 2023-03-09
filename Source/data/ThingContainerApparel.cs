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
        public static readonly List<StuffCategoryDef> Stuffs = new List<StuffCategoryDef>();
        public static readonly List<BodyPartGroupDef> BodyParts = new List<BodyPartGroupDef>();

        public static readonly List<AStatProcessor> StatProcessors = new List<AStatProcessor>();

        public CellData[] CachedCells { get; private set; } = { };

        public float CachedSortingWeight { get; private set; }

        private ThingContainerApparel(ThingDef thingDef)
        {
            Def = thingDef;

            if (thingDef.MadeFromStuff)
            {
                // todo! деструктуризация по материалу
                // todo! вычислить лучший материал по выбранным параметрам сортировки
                DefaultThing = ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef));
            }
            else
            {
                DefaultThing = ThingMaker.MakeThing(thingDef);
            }
        }

        public static void ClearThingDefs()
        {
            AllApparels.Clear();
            Layers.Clear();
            Stuffs.Clear();
            BodyParts.Clear();
            StatProcessors.Clear();
        }

        public static void TryToAddThingDef(ThingDef thingDef)
        {
            if (thingDef.IsApparel)
            {
                try
                {
                    AllApparels.Add(new ThingContainerApparel(thingDef));
                }
                catch (NullReferenceException e)
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
                    .GroupBy(it => it.GetStatDef().defName)
                    .Select(it => it.First())
                    .OrderBy(it => it.GetStatDef().label)
            );
        }

        public override bool CheckForFilters()
        {
            // if have any 'ON' state - the thing should contain ALL of it
            if (BestApparel.Config.EnabledLayers.Count > 0)
            {
                if (!Def.apparel.layers.All(l => BestApparel.Config.EnabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (BestApparel.Config.DisabledLayers.Count > 0)
            {
                if (Def.apparel.layers.Any(l => BestApparel.Config.DisabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'ON' state - the thing should contain ANY of it
            if (BestApparel.Config.EnabledBodyParts.Count > 0)
            {
                if (!Def.apparel.bodyPartGroups.Any(l => BestApparel.Config.EnabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (BestApparel.Config.DisabledBodyParts.Count > 0)
            {
                if (Def.apparel.bodyPartGroups.Any(l => BestApparel.Config.DisabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            return true;
        }

        public override void MakeCache()
        {
            CachedCells = BestApparel.Config.Columns.Apparel //
                .Select(
                    defName =>
                    {
                        var proc = StatProcessors.FirstOrDefault(it => it.GetStatDef().defName == defName);
                        return proc == null ? null : new CellData(proc, DefaultThing);
                    }
                )
                .Where(it => it != null)
                .OrderByDescending(c => c.WeightFactor)
                .ThenBy(c => c.DefLabel)
                .ToArray();
        }

        public void MakeSortingWeightsCache()
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