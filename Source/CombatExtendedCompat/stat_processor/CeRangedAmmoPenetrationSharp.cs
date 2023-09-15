using System.Linq;
using CombatExtended;
using Verse;

namespace BestApparel.CombatExtendedCompat;

public class CeRangedAmmoPenetrationSharp : AStatProcessor
{
    public override float CellWidth => 110;
    public override string[] ActivateWith => new[] { "CE_Ammo" };

    public CeRangedAmmoPenetrationSharp() : base(DefaultStat)
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

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) =>
        GetStatValue(thing).ToStringByStyle(ToStringStyle.FloatTwo) + " " + "CE_mmRHA".Translate();
}