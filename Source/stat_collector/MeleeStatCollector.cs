using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class MeleeStatCollector : IStatCollector
{
    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_AverageArmorPenetration);
        yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_DamageMultiplier);
        yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_CooldownMultiplier);
        yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_AverageDPS);
    }
}