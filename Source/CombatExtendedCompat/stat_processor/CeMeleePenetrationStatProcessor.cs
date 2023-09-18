using System.Linq;
using CombatExtended;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeMeleePenetrationStatProcessor : AStatProcessor
{
    private readonly bool _sharp;

    public CeMeleePenetrationStatProcessor(bool sharp, IStatCollector collector) : base(DefaultStat, collector)
    {
        _sharp = sharp;
    }

    public override string GetDefLabel() => _sharp ? "CE_DescSharpPenetration".Translate() : "CE_DescBluntPenetration".Translate();

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

    public override string GetStatValueFormatted(Thing thing) =>
        GetStatValue(thing).ToStringByStyle(ToStringStyle.FloatTwo) + (BestApparel.Config.CePenetrationShortValue ? "" : " " + (_sharp ? "CE_mmRHA" : "CE_MPa").Translate());
}