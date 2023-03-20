using System.Linq;
using BestApparel.stat_processor;
using CombatExtended;
using RimWorld;
using Verse;

namespace BestApparel.compatibility.stat_processor;

public class CeMeleePenetrationStatProcessor : AStatProcessor
{
    private readonly bool _sharp;

    public CeMeleePenetrationStatProcessor(bool sharp) : base(DefaultStat)
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