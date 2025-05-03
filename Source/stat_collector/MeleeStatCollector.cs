using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class MeleeStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new EquippedOffsetStatProcessor(StatDefOf.MeleeWeapon_DamageMultiplier, this);
        yield return new EquippedOffsetStatProcessor(StatDefOf.MeleeWeapon_CooldownMultiplier, this);
        yield return new EquippedOffsetStatProcessor(StatDefOf.MeleeWeapon_AverageDPS, this);
    }
}