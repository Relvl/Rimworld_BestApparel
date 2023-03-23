using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

public class EquippedStatCollector : IStatCollector
{
    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        if (thing.def.equippedStatOffsets is null) yield break;
        foreach (var statOffset in thing.def.equippedStatOffsets)
        {
            var stat = statOffset?.stat;
            if (stat == null) continue;
            var proc = new CommonStatProcessor(stat);
            if (!proc.IsValueDefault(thing))
            {
                yield return proc;
            }
        }
    }
}

public class CommonStatProcessor : AStatProcessor
{
    public CommonStatProcessor(StatDef def) : base(def)
    {
    }

    public override float GetStatValue(Thing thing) => StatWorker.StatOffsetFromGear(thing, Def);

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => GetStatValueFormatted(Def, GetStatValue(thing), forceUnformatted);

    public override int GetHashCode() => Def.GetHashCode();
}