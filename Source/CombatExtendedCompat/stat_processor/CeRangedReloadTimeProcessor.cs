using System;
using System.Globalization;
using CombatExtended;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedReloadTimeProcessor(IStatCollector collector) : AStatProcessor(DefaultStat, collector)
{
    public override string GetDefName() => "CeRangedReloadTime";
    public override string GetDefLabel() => TranslationCache.StatCeRangedReloadTime.Text;
    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) < Config.DefaultTolerance;

    public override float GetStatValue(Thing thing)
    {
        try
        {
            return thing.def.GetCompProperties<CompProperties_AmmoUser>()?.reloadTime ?? thing.GetStatValue(StatDef);
        }
        catch (Exception e)
        {
            Log.Warning($"Can't get stat 'CeRangedReloadTime' @ {thing.def.defName}\n\t" + e.Message);
            return 0;
        }
    }

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.InvariantCulture);
}