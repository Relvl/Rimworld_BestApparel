using System.Globalization;
using CombatExtended;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedReloadTimeProcessor : AStatProcessor
{
    public CeRangedReloadTimeProcessor(IStatCollector collector) : base(DefaultStat, collector)
    {
    }

    public override string GetDefName() => "CeRangedReloadTime";
    public override string GetDefLabel() => TranslationCache.StatCeRangedReloadTime.Text;
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) < Config.DefaultTolerance;

    public override float GetStatValue(Thing thing) => thing.def.GetCompProperties<CompProperties_AmmoUser>()?.reloadTime ?? thing.GetStatValue(StatDef);

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.InvariantCulture);
}