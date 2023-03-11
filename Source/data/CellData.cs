using System.Collections.Generic;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.data;

public class CellData
{
    public readonly string DefName;
    public readonly float ValueRaw;
    public readonly string Value;
    public readonly bool IsEmpty;
    public readonly float WeightFactor;
    public readonly string DefLabel;
    public readonly List<string> Tooltips = new();
    public readonly float NormalizedWeight;

    public CellData(AStatProcessor processor, Thing defaultThing, float weightFactor, float normalizedWeight)
    {
        WeightFactor = weightFactor;
        NormalizedWeight = normalizedWeight;

        DefName = processor.GetDefName();
        IsEmpty = processor.IsValueDefault(defaultThing);
        ValueRaw = processor.GetStatValue(defaultThing);
        Value = IsEmpty ? "---" : processor.GetStatValueFormatted(defaultThing, true);
        DefLabel = processor.GetDefLabel();

        if (!IsEmpty)
        {
            Tooltips.Add($"{processor.GetDefLabel()}: {Value}");
        }

        if (Prefs.DevMode)
        {
            Tooltips.Add($"Raw: {ValueRaw}, Default: {processor.GetStatDef().defaultBaseValue}");
        }
    }
}