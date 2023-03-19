using System.Collections.Generic;
using BestApparel.compatibility.stat_processor;
using BestApparel.stat_processor;
using CombatExtended;
using RimWorld;
using Verse;

namespace BestApparel.compatibility;

public static class CombatExtendedCompat
{
    public static IEnumerable<AStatProcessor> GetMeleeStats(ThingDef thingDef)
    {
        if (thingDef.tools != null && Config.IsCeLoaded)
        {
            yield return new CeMeleeDamageStatProcessor();
            yield return new CeMeleePenetration(true);
            yield return new CeMeleePenetration(false);
            yield return new CommonStatProcessor(CE_StatDefOf.MeleePenetrationFactor);
        }
    }

    public static IEnumerable<AStatProcessor> GetRangedStats(ThingDef thingDef)
    {
        yield return new CommonStatProcessor(CE_StatDefOf.MagazineCapacity);
        yield return new CommonStatProcessor(CE_StatDefOf.MuzzleFlash);
        yield return new CommonStatProcessor(CE_StatDefOf.ReloadSpeed);
        yield return new CommonStatProcessor(CE_StatDefOf.AimingAccuracy);
        yield return new CommonStatProcessor(CE_StatDefOf.SwayFactor);
        yield return new CommonStatProcessor(CE_StatDefOf.ShotSpread);
        yield return new CommonStatProcessor(CE_StatDefOf.NightVisionEfficiency_Weapon);
        yield return new CommonStatProcessor(CE_StatDefOf.Suppressability);
        yield return new CeRangedAmmoType();
    }
}