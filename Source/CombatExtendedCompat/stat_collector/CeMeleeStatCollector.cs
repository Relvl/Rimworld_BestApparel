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
        yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
        yield return new CommonStatProcessor(StatDef.Named("MeleeCounterParryBonus"));
        if (thing.def.tools != null)
        {
            yield return new CeMeleeDamageStatProcessor();
            yield return new CeMeleePenetrationStatProcessor(true);
            yield return new CeMeleePenetrationStatProcessor(false);
            yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
        }
    }
}