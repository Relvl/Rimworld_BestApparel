using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedRecoilPatternProcessor : AStatProcessor
{
    public CeRangedRecoilPatternProcessor(IStatCollector collector) : base(DefaultStat, collector)
    {
    }

    public override string GetDefName() => "CeRangedRecoilPattern";
    public override string GetDefLabel() => TranslationCache.StatCeRangedRecoilPattern.Text;
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) < 0;

    public override float GetStatValue(Thing thing)
    {
        var verb = thing.def.Verbs.FirstOrDefault(it => it is VerbPropertiesCE);
        if (verb is not VerbPropertiesCE verbPropertiesCe) return -1;
        return (byte)verbPropertiesCe.recoilPattern;
    }

    public override string GetStatValueFormatted(Thing thing)
    {
        var verb = thing.def.Verbs.FirstOrDefault(it => it is VerbPropertiesCE);
        if (verb is not VerbPropertiesCE verbPropertiesCe) return "";
        return verbPropertiesCe.recoilPattern.ToString().Translate();
    }
}