using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class CeRangedStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
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
        yield return new FuncStatProcessor(weapon => weapon.def.Verbs.FirstOrDefault()?.range ?? 0, "Range");
    }
}