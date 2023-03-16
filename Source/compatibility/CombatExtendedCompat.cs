using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using CombatExtended;
using RimWorld;
using Verse;

namespace BestApparel.compatibility;

public static class CombatExtendedCompat
{
    public static IEnumerable<AStatProcessor> GetMeleeStats(ThingDef thingDef)
    {
        if (thingDef.tools != null && Config.IsCeLoaded)
        {
            yield return new CeMeleeDamageStatProcessor();
            yield return new CeMeleePenetration(true);
            yield return new CeMeleePenetration(false);
            yield return new CommonStatProcessor(CE_StatDefOf.MeleePenetrationFactor);
        }
    }
}

public class CeMeleeDamageStatProcessor : AStatProcessor
{
    public CeMeleeDamageStatProcessor() : base(DefaultStat)
    {
    }

    public override string GetDefLabel() => TranslationCache.StatMeleeAvgDamage.Text;

    public override string GetDefName() => "Stat_Melee_Avg_Damage";

    public override bool IsValueDefault(Thing thing) => thing.def.tools is { Count: 0 };

    private (float, float, float) GetDamage(Thing thing)
    {
        // CombatExtended.StatWorker_MeleeDamage.GetFinalDisplayValue

        var minFactor = 0.5f;
        var maxFactor = 1.5f;
        if (thing.ParentHolder is Pawn_EquipmentTracker parentHolder)
        {
            minFactor = StatWorker_MeleeDamageBase.GetDamageVariationMin(parentHolder.pawn);
            maxFactor = StatWorker_MeleeDamageBase.GetDamageVariationMax(parentHolder.pawn);
        }

        var minValue = float.MaxValue;
        var maxValue = 0.0f;
        foreach (var tool in thing.def.tools)
        {
            var adjustedDamage = 0f;
            if (tool is ToolCE toolCe)
            {
                adjustedDamage = StatWorker_MeleeDamageBase.GetAdjustedDamage(toolCe, thing);
            }

            if (adjustedDamage > maxValue) maxValue = adjustedDamage;
            if (adjustedDamage < minValue) minValue = adjustedDamage;
        }

        return (minValue * minFactor, (minValue * minFactor + maxValue * maxFactor) / 2, maxValue * maxFactor);
    }

    public override float GetStatValue(Thing thing) => GetDamage(thing).Item2;

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
    {
        var (min, _, max) = GetDamage(thing);
        return $"{min.ToStringByStyle(ToStringStyle.FloatMaxTwo)} - {max.ToStringByStyle(ToStringStyle.FloatMaxTwo)}";
    }
}

public class CeMeleePenetration : AStatProcessor
{
    private readonly bool _sharp;

    public CeMeleePenetration(bool sharp) : base(DefaultStat)
    {
        _sharp = sharp;
    }

    public override string GetDefLabel() => (_sharp ? TranslationCache.StatMeleePenetrationSharp : TranslationCache.StatMeleePenetrationBlunt).Text;

    public override string GetDefName() => _sharp ? "CE_DescSharpPenetration" : "CE_DescBluntPenetration";

    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0f;

    public override float GetStatValue(Thing thing)
    {
        if (thing.def.tools is null) return 0f;
        var toolChanceFactor = thing.def.tools.Sum(tool => tool.chanceFactor);
        var penetration = 0.0f;
        foreach (var tool in thing.def.tools)
        {
            if (tool is ToolCE toolCe)
            {
                penetration += tool.chanceFactor / toolChanceFactor * (_sharp ? toolCe.armorPenetrationSharp : toolCe.armorPenetrationBlunt);
            }
        }

        return thing.GetStatValue(CE_StatDefOf.MeleePenetrationFactor) * penetration;
    }

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
    {
        var penetration = GetStatValue(thing);
        return penetration.ToStringByStyle(ToStringStyle.FloatMaxTwo);
    }
}