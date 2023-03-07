using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_selector;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public class ApparelThing : ComparableThing
    {
        public static readonly List<ApparelThing> AllApparels = new List<ApparelThing>();
        public static readonly List<ApparelLayerDef> Layers = new List<ApparelLayerDef>();
        public static readonly List<StuffCategoryDef> Stuffs = new List<StuffCategoryDef>();
        public static readonly List<BodyPartGroupDef> BodyParts = new List<BodyPartGroupDef>();
        public static readonly List<StatDef> Stats = new List<StatDef>();
        public static readonly List<IStatSelector> StatSelectors = new List<IStatSelector>();

        private ApparelThing(ThingDef thingDef)
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
        }

        public static void TryToAddThingDef(ThingDef thingDef)
        {
            if (thingDef.IsApparel)
            {
                AllApparels.Add(new ApparelThing(thingDef));
            }
        }

        public static void FinalyzeThingDefs()
        {
            Layers.Clear();
            Layers.AddRange(
                AllApparels //
                    .SelectMany(it => it.Def.apparel.layers)
                    .Where(it => it != null)
                    .Distinct()
            );

            Stuffs.Clear();
            Stuffs.AddRange(
                AllApparels //
                    .Where(it => it.Def.stuffProps?.categories != null)
                    .SelectMany(it => it.Def.stuffProps?.categories)
                    .Distinct()
            );

            BodyParts.Clear();
            BodyParts.AddRange(AllApparels.SelectMany(it => it.Def.apparel.bodyPartGroups).Where(it => it != null).Distinct());

            var equippedStatOffsets = AllApparels // 
                .Where(it => it.Def.equippedStatOffsets != null)
                .SelectMany(it => it.Def.equippedStatOffsets)
                .Where(it => it != null)
                .Select(it => it.stat)
                .Distinct()
                .OrderBy(it => it.label)
                .ToList();
            var statBases = AllApparels //
                .Where(it => it.Def.statBases != null)
                .SelectMany(it => it.Def.statBases)
                .Where(it => it != null)
                .Select(it => it.stat)
                .Distinct()
                .OrderBy(it => it.label)
                .ToList();

            Stats.Clear();
            StatSelectors.Clear();
            equippedStatOffsets.ForEach(
                it =>
                {
                    Stats.Add(it);
                    StatSelectors.Add(new StatSelectorEquippedOffset(it.defName));
                }
            );
            statBases.ForEach(
                it =>
                {
                    Stats.Add(it);
                    StatSelectors.Add(new StatSelectorStatBases(it.defName));
                }
            );
            // todo custom stats here <- StatSelectorCommon.cs + ???
        }
    }
}