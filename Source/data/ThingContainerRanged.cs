using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public class ThingContainerRanged : AThingContainer
    {
        public static readonly List<ThingContainerRanged> AllRanged = new List<ThingContainerRanged>();
        public static readonly List<WeaponClassDef> WeaponClasses = new List<WeaponClassDef>();
        public static readonly List<AStatProcessor> StatProcessors = new List<AStatProcessor>();
        public static readonly List<ThingCategoryDef> Categories = new List<ThingCategoryDef>();
        public static readonly List<StuffCategoryDef> Stuffs = new List<StuffCategoryDef>();

        private ThingContainerRanged(ThingDef thingDef) : base(thingDef)
        {
        }

        public static void ClearThingDefs()
        {
            AllRanged.Clear();
            WeaponClasses.Clear();
            Categories.Clear();
            Stuffs.Clear();
            StatProcessors.Clear();
        }

        public static void TryToAddThingDef(ThingDef thingDef)
        {
            if (thingDef.IsWeapon)
            {
                if (thingDef.weaponTags == null || !thingDef.weaponTags.Contains("Gun")) return;

                try
                {
                    if (Prefs.DevMode && BestApparel.Config.DoLogThingsLoading)
                    {
                        Log.Message($"--> BestApparel process thingDef: {thingDef.defName}");
                    }

                    AllRanged.Add(new ThingContainerRanged(thingDef));
                }
                catch (Exception e)
                {
                    Log.Warning($"Can not make ranged cache for ThingDef '{thingDef.defName}' -> {e.Message}");
                }
            }
        }

        public static void FinalyzeThingDefs()
        {
            WeaponClasses.AddRange(
                AllRanged //
                    .Where(it => it.Def.weaponClasses != null)
                    .SelectMany(it => it.Def.weaponClasses)
                    .GroupBy(it => it.defName)
                    .Select(it => it.First())
            );
            Stuffs.AddRange(
                AllRanged //
                    .Where(it => it.Def.stuffProps?.categories != null)
                    .SelectMany(it => it.Def.stuffProps?.categories)
                    .Distinct()
            );
            Categories.AddRange(
                AllRanged //
                    .Where(it => it.Def.thingCategories != null)
                    .SelectMany(it => it.Def.thingCategories)
                    .GroupBy(it => it.defName)
                    .Select(it => it.First())
            );

            // todo! CE flags from Verb_ShootCE

            var tmpStatProcessors = new List<AStatProcessor>();
            // Base stats
            tmpStatProcessors.AddRange(
                AllRanged.SelectMany(
                    apparel => DefDatabase<StatDef>.AllDefs //
                        .Where(def => def.Worker.ShouldShowFor(StatRequest.For(apparel.DefaultThing)) && !def.Worker.IsDisabledFor(apparel.DefaultThing))
                        .Select(def => new BaseStatProcessor(def))
                        .Where(processor => !processor.IsValueDefault(apparel.DefaultThing))
                )
            );

            if (Config.IsCeLoaded)
            {
                // Verb_LaunchProjectileCE
                // Verb_LaunchProjectileChangeAble
                // Verb_LaunchProjectileStaticCE
                // Verb_MarkForArtillery
                // Verb_MeleeAttackCE
                // Verb_ShootCE
                // Verb_ShootCEOneUse (grenades, javelin)
                // Verb_ShootFlareCE
                // Verb_ShootMortarCE

                AllRanged //
                    .Where(r => r.Def.Verbs != null)
                    .SelectMany(r => r.Def.Verbs)
                    .Where(v => v.verbClass.FullName == "CombatExtended.Verb_ShootCE")
                    .ToList()
                    .ForEach(verb => { tmpStatProcessors.Add(new FuncStatProcessor(thing => verb.range, "Ability_Range.label")); });
                tmpStatProcessors.Add(new CommonStatProcessor(StatDef.Named("MagazineCapacity")));
            }

            StatProcessors.AddRange(
                tmpStatProcessors //
                    .GroupBy(it => it.GetDefName())
                    .Select(it => it.First())
                    .OrderBy(it => it.GetDefLabel())
            );
        }

        public override void MakeCache()
        {
            CachedCells = BestApparel.Config.Columns.Ranged //
                .Select(
                    defName =>
                    {
                        var proc = StatProcessors.FirstOrDefault(it => it.GetDefName() == defName);
                        return proc == null ? null : new CellData(proc, DefaultThing, BestApparel.Config.Sorting.Ranged[proc.GetDefName()] + Config.MaxSortingWeight);
                    }
                )
                .Where(it => it != null)
                .OrderByDescending(c => c.WeightFactor)
                .ThenBy(c => c.DefLabel)
                .ToArray();
        }

        public override bool CheckForFilters()
        {
            if (!BestApparel.Config.RangedCategories.IsCollectionAllowed(Def.thingCategories)) return false;
            if (!BestApparel.Config.RangedTypes.IsCollectionAllowed(Def.weaponClasses)) return false;
            return true;
        }

        public override void MakeSortingWeightsCache()
        {
            foreach (var defName in BestApparel.Config.Columns.Ranged)
            {
                var thisCell = CachedCells.FirstOrDefault(c => c.DefName == defName);
                if (thisCell == null) continue;

                var rawValues = DataProcessor.CachedRanged //
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