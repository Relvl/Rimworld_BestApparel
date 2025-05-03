using System;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class EquippedOffsetStatProcessor(StatDef statDef, IStatCollector collector) : AStatProcessor(statDef, collector)
{
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0f;

    public override float GetStatValue(Thing thing)
    {
        try
        {
            return StatWorker.StatOffsetFromGear(thing, StatDef);
        }
        catch (Exception e)
        {
            Log.Error($"Can't get value for stat - thing:{thing?.def?.defName}, stat:{this.StatDef?.defName}\n{e}");
            return 0f;
        }
    }

    public override string GetStatValueFormatted(Thing thing) => StatDef.ValueToString(GetStatValue(thing), ToStringNumberSense.Offset, !StatDef.formatString.NullOrEmpty());

    public override int GetHashCode() => StatDef.GetHashCode();
}