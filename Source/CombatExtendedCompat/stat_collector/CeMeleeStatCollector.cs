using System.Collections.Generic;
using System.Linq;
using CombatExtended;
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
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.ToughnessRating, this);
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.MeleeCritChance, this);
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.MeleeDodgeChance, this);
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.MeleeParryChance, this);
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.MeleePenetrationFactor, this);
        yield return new EquippedOffsetStatProcessor(CE_StatDefOf.MeleeCounterParryBonus, this);
        if (!thing.def.tools.NullOrEmpty() && thing.def.tools.All(t => t is ToolCE))
        {
            yield return new CeMeleeDamageStatProcessor(this);
            yield return new CeMeleePenetrationStatProcessor(true, this);
            yield return new CeMeleePenetrationStatProcessor(false, this);
        }
    }
}