using System.Collections.Generic;
using System.Linq;
using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedAmmoPenetrationSharp : AStatProcessor
{
    public override float CellWidth => 110;
    public override IEnumerable<string> ActivateWith => new[] { "CE_Ammo" };

    public CeRangedAmmoPenetrationSharp(IStatCollector collector) : base(DefaultStat, collector)
    {
    }

    public override string GetDefName() => "CE_AmmoPenetrationSharp";
    public override string GetDefLabel() => "CE_DescSharpPenetration".Translate();
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0;

    public override float GetStatValue(Thing thing)
    {
        var ammoUser = thing.TryGetComp<CompAmmoUser>();
        var link = CeAmmoCellData.GetLink(ammoUser);
        if (link is null) return thing.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetArmorPenetration(thing) ?? 0f;
        if (link.projectile.projectile is ProjectilePropertiesCE projProps)
            return projProps.armorPenetrationSharp;
        return link.projectile.projectile.GetArmorPenetration(thing);
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToStringByStyle(ToStringStyle.FloatTwo) + " " + "CE_mmRHA".Translate();
}