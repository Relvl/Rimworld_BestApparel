using System.Collections.Generic;
using System.Linq;
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
        yield return new EquippedOffsetStatProcessor(StatDef.Named("ReloadSpeed"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("AimingAccuracy"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("SwayFactor"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("ShotSpread"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("NightVisionEfficiency_Weapon"), this);
        yield return new EquippedOffsetStatProcessor(StatDef.Named("Suppressability"), this);
        yield return new CeAmmoStatProcessor(this);
        yield return new CeRangedDamageStatProcessor(this);
        yield return new CeRangedAmmoPenetrationSharp(this);
        yield return new CeRangedAmmoPenetrationBlunt(this);
        yield return new CeRangedRecoilPatternProcessor(this);
        yield return new CeRangedWarmupTimeProcessor(this);
        yield return new CeRangedMuzzleFlashScaleProcessor(this);
        yield return new CeRangedMagazineSizeProcessor(this);
        yield return new CeRangedReloadTimeProcessor(this);
        yield return new CeRangedAmmoSpeedProcessor(this);
        yield return new FuncStatProcessor(weapon => weapon.def.Verbs.FirstOrDefault()?.range ?? 0, "Range", this);
    }
}