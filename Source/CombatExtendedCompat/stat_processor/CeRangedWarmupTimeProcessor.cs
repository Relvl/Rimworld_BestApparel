using System.Globalization;
using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedWarmupTimeProcessor : AStatProcessor
{
    public CeRangedWarmupTimeProcessor(IStatCollector collector) : base(DefaultStat, collector)
    {
    }

    public override string GetDefName() => "CeRangedWarmupTime";
    public override string GetDefLabel() => TranslationCache.StatCeRangedWarmupTime.Text;
    public override bool IsValueDefault(Thing thing) => false;

    public override float GetStatValue(Thing thing)
    {
        var verb = thing.def.Verbs.FirstOrDefault(it => it is VerbPropertiesCE);
        if (verb is not VerbPropertiesCE verbPropertiesCe) return -1;
        return verbPropertiesCe.warmupTime;
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.InvariantCulture);
}