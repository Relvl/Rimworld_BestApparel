using System;
using BestApparel;
using RimWorld;
using SurvivalToolsLite;
using Verse;

namespace SurvivalToolsLightCompat.stat_processor;

public class StlToolStatProcessor : AStatProcessor
{
    public StlToolStatProcessor(StatDef def) : base(def)
    {
    }

    public override string GetDefName() => $"stl.{base.GetDefName()}";
    public override string GetDefLabel() => $"Tool: {base.GetDefLabel()}";

    public override bool IsValueDefault(Thing thing) => Math.Abs(GetStatValue(thing) - 1f) < 0.00001;

    public override float GetStatValue(Thing thing) => GetModifier(thing)?.value ?? 1f;

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => $"x{GetStatValue(thing) * 100}%";

    private StatModifier GetModifier(Thing thing) =>
        thing.def.HasModExtension<SurvivalToolProperties>() ? thing.def.GetModExtension<SurvivalToolProperties>().baseWorkStatFactors.Find(f => f.stat == Def) : null;
}