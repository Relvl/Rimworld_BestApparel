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
        yield return new EquippedOffsetStatCollector(StatDef.Named("MagazineCapacity"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("MuzzleFlash"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("ReloadSpeed"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("AimingAccuracy"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("SwayFactor"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("ShotSpread"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("NightVisionEfficiency_Weapon"), this);
        yield return new EquippedOffsetStatCollector(StatDef.Named("Suppressability"), this);
        yield return new CeAmmoStatProcessor(this);
        yield return new CeRangedDamageStatProcessor(this);
        yield return new CeRangedAmmoPenetrationSharp(this);
        yield return new CeRangedAmmoPenetrationBlunt(this);
        yield return new FuncStatProcessor(weapon => weapon.def.Verbs.FirstOrDefault()?.range ?? 0, "Range", this);
    }
}