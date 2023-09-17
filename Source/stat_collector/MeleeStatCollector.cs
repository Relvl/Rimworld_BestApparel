using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class MeleeStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new EquippedOffsetStatCollector(StatDefOf.MeleeWeapon_AverageArmorPenetration, this);
        yield return new EquippedOffsetStatCollector(StatDefOf.MeleeWeapon_DamageMultiplier, this);
        yield return new EquippedOffsetStatCollector(StatDefOf.MeleeWeapon_CooldownMultiplier, this);
        yield return new EquippedOffsetStatCollector(StatDefOf.MeleeWeapon_AverageDPS, this);
    }
}