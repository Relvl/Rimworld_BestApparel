using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public class ThingContainerApparel : AThingContainer
    {
        private static readonly string[] IgnoredStatDefNames = { };

        public static readonly List<ThingContainerApparel> AllApparels = new List<ThingContainerApparel>();
        public static readonly List<ApparelLayerDef> Layers = new List<ApparelLayerDef>();
        public static readonly List<StuffCategoryDef> Stuffs = new List<StuffCategoryDef>();
        public static readonly List<BodyPartGroupDef> BodyParts = new List<BodyPartGroupDef>();

        public static readonly HashSet<AStatProcessor> StatProcessors = new HashSet<AStatProcessor>();

        public CellData[] CachedCells { get; private set; } = { };

        public float CachedSortingWeight { get; private set; } = 0f;

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

            BodyParts.AddRange(AllApparels.SelectMany(it => it.Def.apparel.bodyPartGroups).Where(it => it != null).Distinct());

            var temStatProcessors = new List<AStatProcessor>();
            foreach (var apparel in AllApparels)
            {
                // Base stats
                temStatProcessors.AddRange(
                    DefDatabase<StatDef>.AllDefs //
                        .Where(st => st.Worker.ShouldShowFor(StatRequest.For(apparel.DefaultThing)) && !st.Worker.IsDisabledFor(apparel.DefaultThing))
                        .Select(st => new StatProcessorBaseStat(st))
                        .Where(it => it.GetStatValue(apparel.DefaultThing) != 0)
                );

                // Equipped stats
                if (apparel.Def.equippedStatOffsets != null)
                {
                    foreach (var statModifier in apparel.Def.equippedStatOffsets)
                    {
                        if (statModifier == null) continue;
                        var proc = new StatProcessorCommon(statModifier.stat);
                        if (proc.GetStatValue(apparel.DefaultThing) == 0) continue;
                        temStatProcessors.Add(proc);
                    }
                }
            }

            StatProcessors.AddRange(
                temStatProcessors //
                    .GroupBy(it => it.GetStatDef().defName)
                    .Select(it => it.First())
                    .OrderBy(it => it.GetStatDef().label)
            );
        }

        public static bool CheckThingForFilters(ThingContainerApparel it)
        {
            // if have any 'ON' state - the thing should contain ALL of it
            if (BestApparel.Config.EnabledLayers.Count > 0)
            {
                if (!it.Def.apparel.layers.All(l => BestApparel.Config.EnabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (BestApparel.Config.DisabledLayers.Count > 0)
            {
                if (it.Def.apparel.layers.Any(l => BestApparel.Config.DisabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'ON' state - the thing should contain ANY of it
            if (BestApparel.Config.EnabledBodyParts.Count > 0)
            {
                if (!it.Def.apparel.bodyPartGroups.Any(l => BestApparel.Config.EnabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (BestApparel.Config.DisabledBodyParts.Count > 0)
            {
                if (it.Def.apparel.bodyPartGroups.Any(l => BestApparel.Config.DisabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            return true;
        }

        public override void MakeCache()
        {
            CachedCells = BestApparel.Config.Columns.Apparel.Select(
                    defName =>
                    {
                        var proc = StatProcessors.FirstOrDefault(it => it.GetStatDef().defName == defName);
                        if (proc == null) return null;

                        var value = proc.GetStatValue(DefaultThing);

                        if (Math.Abs(value - proc.GetStatDef().defaultBaseValue) > 0.00001)
                        {
                            var formattedValue = AStatProcessor.GetStatValueFormatted(proc.GetStatDef(), value, true);
                            return new CellData(
                                defName,
                                formattedValue,
                                new List<string>(new[] { $"{proc.GetStatDef().label}: {formattedValue}" }),
                                false,
                                value,
                                proc.GetStatDef()
                            );
                        }
                        else
                        {
                            const string formattedValue = "---";
                            return new CellData(defName, formattedValue, new List<string>(), false, value, proc.GetStatDef());
                        }
                    }
                )
                .Where(it => it != null)
                .ToArray();
        }

        public void MakeSortingWeightsCache()
        {
            // здесь найти процент от мин-макса, умножить на вес, а в сорте вернуть сумму всех процентов
            foreach (var defName in BestApparel.Config.Columns.Apparel)
            {
                var thisCell = CachedCells.FirstOrDefault(c => c.DefName == defName);
                if (thisCell == null) continue;
                var proc = StatProcessors.FirstOrDefault(it => it.GetStatDef().defName == defName);
                if (proc == null) continue;

                var rawValues = DataProcessor.CachedApparels //
                    .Select(a => a.CachedCells.FirstOrDefault(c => c.DefName == defName))
                    .Where(v => v != null)
                    .Select(c => c.ValueRaw)
                    .ToArray();
                var valueMin = rawValues.Min();
                var valueMax = rawValues.Max();
                var value = proc.GetStatValue(DefaultThing);

                if (!BestApparel.Config.Sorting.Apparel.ContainsKey(defName)) BestApparel.Config.Sorting.Apparel[defName] = 0f;
                thisCell.NormalizedWeight = (value - valueMin) / (valueMax - valueMin);
                if (float.IsNaN(thisCell.NormalizedWeight)) thisCell.NormalizedWeight = 0f;
                thisCell.WeightFactor = BestApparel.Config.Sorting.Apparel[defName] + Config.MaxSortingWeight;
                if (Prefs.DevMode)
                {
                    thisCell.Tooltips.Add($"defName: {thisCell.DefName}");
                    thisCell.Tooltips.Add($"min: {valueMin}, val: {value}, max: {valueMax}, norm: {thisCell.NormalizedWeight}");
                }

                thisCell.Tooltips.Add("BestApparel.Label.RangePercent".Translate(Math.Round(thisCell.NormalizedWeight * 100f, 1), thisCell.WeightFactor));
            }

            CachedSortingWeight = CachedCells.Sum(c => c.NormalizedWeight * c.WeightFactor);
        }
    }
}