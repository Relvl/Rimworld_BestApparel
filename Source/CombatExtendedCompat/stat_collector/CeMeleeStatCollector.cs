using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class CeMeleeStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new EquippedOffsetStatProcessor(StatDef.Named("MeleePenetrationFactor"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("MeleeCounterParryBonus"), this);
        if (thing.def.tools != null)
        {
            yield return new CeMeleeDamageStatProcessor(this);
            yield return new CeMeleePenetrationStatProcessor(true, this);
            yield return new CeMeleePenetrationStatProcessor(false, this);
        }
    }
}