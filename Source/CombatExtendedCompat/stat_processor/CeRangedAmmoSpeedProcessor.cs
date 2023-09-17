using System;
using System.Collections.Generic;
using System.Linq;
using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedAmmoSpeedProcessor : AStatProcessor
{
    public override float CellWidth => 110;
    public override IEnumerable<string> ActivateWith => new[] { "CE_Ammo" };

    public CeRangedAmmoSpeedProcessor(IStatCollector collector) : base(DefaultStat, collector)
    {
    }

    public override string GetDefName() => "CeRangedAmmoSpeed";
    public override string GetDefLabel() => TranslationCache.StatCeRangedAmmoSpeed.Text;

    public override bool IsValueDefault(Thing thing)
    {
        var value = GetStatValue(thing);
        return value <= 0 || Math.Abs(value - 5f) < Config.DefaultTolerance;
    }

    public override float GetStatValue(Thing thing)
    {
        var ammoUser = thing.TryGetComp<CompAmmoUser>();
        if (ammoUser is null) return 0;
        var link = CeAmmoCellData.GetLink(ammoUser);
        return link?.projectile?.projectile?.speed ?? 0;
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToStringByStyle(ToStringStyle.FloatTwo);
}