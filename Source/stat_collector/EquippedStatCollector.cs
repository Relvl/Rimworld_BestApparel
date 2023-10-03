using System.Collections.Generic;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class EquippedStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        if (thing.def.equippedStatOffsets is null) yield break;
        foreach (var statOffset in thing.def.equippedStatOffsets)
        {
            var stat = statOffset?.stat;
            if (stat is null) continue;
            yield return new EquippedOffsetStatProcessor(stat, this);
        }
    }
}