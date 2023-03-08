using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
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
        }

        public static void TryToAddThingDef(ThingDef thingDef)
        {
            if (thingDef.IsApparel)
            {
                AllApparels.Add(new ThingContainerApparel(thingDef));
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

            StatProcessors.Clear();
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
            if (Config.Instance.EnabledLayers.Count > 0)
            {
                if (!it.Def.apparel.layers.All(l => Config.Instance.EnabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (Config.Instance.DisabledLayers.Count > 0)
            {
                if (it.Def.apparel.layers.Any(l => Config.Instance.DisabledLayers.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'ON' state - the thing should contain ANY of it
            if (Config.Instance.EnabledBodyParts.Count > 0)
            {
                if (!it.Def.apparel.bodyPartGroups.Any(l => Config.Instance.EnabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            // if have any 'OFF' state - the thing should not contain it
            if (Config.Instance.DisabledBodyParts.Count > 0)
            {
                if (it.Def.apparel.bodyPartGroups.Any(l => Config.Instance.DisabledBodyParts.Contains(l.defName)))
                {
                    return false;
                }
            }

            return true;
        }

        public override void MakeCache()
        {
            
        }
    }
}