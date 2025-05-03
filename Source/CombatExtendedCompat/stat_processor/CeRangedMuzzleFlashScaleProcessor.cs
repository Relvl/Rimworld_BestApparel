using System.Globalization;
using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedMuzzleFlashScaleProcessor(IStatCollector collector) : AStatProcessor(DefaultStat, collector)
{
    public override string GetDefName() => "CeRangedMuzzleFlashScale";
    public override string GetDefLabel() => TranslationCache.StatCeRangedMuzzleFlashScale.Text;
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) < Config.DefaultTolerance;

    public override float GetStatValue(Thing thing)
    {
        var verb = thing.def.Verbs.FirstOrDefault(it => it is VerbPropertiesCE);
        if (verb is not VerbPropertiesCE verbPropertiesCe) return -1;
        return verbPropertiesCe.muzzleFlashScale;
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.InvariantCulture);
}