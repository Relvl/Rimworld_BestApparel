using System.Collections.Generic;
using BestApparel.compatibility.stat_processor;
using BestApparel.stat_collector;
using BestApparel.stat_processor;
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
            yield return new CeMeleePenetrationStatProcessor(true);
            yield return new CeMeleePenetrationStatProcessor(false);
            yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
        }
    }

    public static IEnumerable<AStatProcessor> GetRangedStats(ThingDef thingDef)
    {
        yield return new CommonStatProcessor(StatDef.Named("MagazineCapacity"));
        yield return new CommonStatProcessor(StatDef.Named("MuzzleFlash"));
        yield return new CommonStatProcessor(StatDef.Named("ReloadSpeed"));
        yield return new CommonStatProcessor(StatDef.Named("AimingAccuracy"));
        yield return new CommonStatProcessor(StatDef.Named("SwayFactor"));
        yield return new CommonStatProcessor(StatDef.Named("ShotSpread"));
        yield return new CommonStatProcessor(StatDef.Named("NightVisionEfficiency_Weapon"));
        yield return new CommonStatProcessor(StatDef.Named("Suppressability"));
        yield return new CeRangedDamageStatProcessor();
    }

    public static void TryToLoadAmmo(Thing thing)
    {
        CeRangedDamageStatProcessor.TryToLoadAmmo(thing);
    }
}