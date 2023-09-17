using System;
using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class EquippedOffsetStatCollector : AStatProcessor
{
    public EquippedOffsetStatCollector(StatDef def, IStatCollector collector) : base(def, collector)
    {
    }

    public override float GetStatValue(Thing thing)
    {
        try
        {
            return StatWorker.StatOffsetFromGear(thing, Def);
        }
        catch (Exception e)
        {
            Log.Error($"Can't get value for stat - thing:{thing?.def?.defName}, stat:{this.Def?.defName}\n{e}");
            return 0f;
        }
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValueFormatted(Def, GetStatValue(thing));

    public override int GetHashCode() => Def.GetHashCode();
}