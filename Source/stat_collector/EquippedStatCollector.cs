using System.Collections.Generic;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
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