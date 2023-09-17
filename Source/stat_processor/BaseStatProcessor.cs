using System;
using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class BaseStatProcessor : AStatProcessor
{
    public BaseStatProcessor(StatDef statDef, IStatCollector collector) : base(statDef, collector)
    {
    }

    public override bool IsValueDefault(Thing thing)
    {
        if (StatDef.alwaysHide) return true;
        var value = GetStatValue(thing);
        return Math.Abs(StatDef.hideAtValue - value) < Config.DefaultTolerance || Math.Abs(StatDef.defaultBaseValue - value) < Config.DefaultTolerance;
    }

    public override float GetStatValue(Thing thing) => StatDef.Worker.GetValue(thing);

    public override string GetStatValueFormatted(Thing thing) => StatDef.ValueToString(GetStatValue(thing), ToStringNumberSense.Absolute, !StatDef.formatString.NullOrEmpty());

    public override int GetHashCode() => StatDef.GetHashCode();
}