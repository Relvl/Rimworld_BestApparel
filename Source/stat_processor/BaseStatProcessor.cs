using System;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class BaseStatProcessor(StatDef statDef, IStatCollector collector) : AStatProcessor(statDef, collector)
{
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