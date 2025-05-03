using System.Collections.Generic;
using System.Linq;
using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedDamageStatProcessor(IStatCollector collector) : AStatProcessor(DefaultStat, collector)
{
    public override IEnumerable<string> ActivateWith => new[] { "CE_Ammo" };

    public override string GetDefName() => "Ranged_Damage";

    public override string GetDefLabel() => TranslationCache.StatCERangedDamage.Text;

    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0;

    public override float GetStatValue(Thing thing)
    {
        var ammoUser = thing.TryGetComp<CompAmmoUser>();
        var link = CeAmmoCellData.GetLink(ammoUser);
        if (link is null) return thing.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetDamageAmount(thing) ?? -1;

        float damageLabel = link.projectile.projectile.GetDamageAmount(thing);

        if (link.projectile.projectile is ProjectilePropertiesCE projProps)
        {
            if (!projProps.secondaryDamage.NullOrEmpty())
            {
                damageLabel += projProps.secondaryDamage.Sum(secondaryDamage => secondaryDamage.amount * secondaryDamage.chance);
            }

            if (projProps.pelletCount > 1)
            {
                damageLabel *= projProps.pelletCount;
            }
        }

        return damageLabel;
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToStringByStyle(ToStringStyle.Integer);
}