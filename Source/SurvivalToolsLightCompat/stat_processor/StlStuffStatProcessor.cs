using System;
using BestApparel;
using RimWorld;
using SurvivalToolsLite;
using Verse;

namespace SurvivalToolsLightCompat.stat_processor;

public class StlStuffStatProcessor : AStatProcessor
{
    public StlStuffStatProcessor(StatDef statDef, IStatCollector collector) : base(statDef, collector)
    {
    }

    public override string GetDefName() => $"stl.{base.GetDefName()}";
    public override string GetDefLabel() => $"Tool: {base.GetDefLabel()}";

    public override bool IsValueDefault(Thing thing) => Math.Abs(GetStatValue(thing) - 1f) < Config.DefaultTolerance;

    public override float GetStatValue(Thing thing) => GetModifier(thing)?.value ?? 1f;

    public override string GetStatValueFormatted(Thing thing) => $"x{GetStatValue(thing) * 100}%";

    private StatModifier GetModifier(Thing thing) =>
        thing.def.HasModExtension<StuffPropsTool>() ? thing.def.GetModExtension<StuffPropsTool>().toolStatFactors.Find(f => f.stat == StatDef) : null;
}