using System.Collections.Generic;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.data;

public class CellData
{
    public readonly string DefName;
    public readonly string Value;
    public readonly bool IsEmpty;
    public readonly float WeightFactor;
    public readonly string DefLabel;
    public readonly List<string> Tooltips = new();

    public float NormalizedWeight { get; set; }

    public CellData(AStatProcessor processor, Thing defaultThing, float weightFactor, float normalizedWeight)
    {
        WeightFactor = weightFactor;
        NormalizedWeight = normalizedWeight;

        DefName = processor.GetDefName();
        IsEmpty = processor.IsValueDefault(defaultThing);
        Value = IsEmpty ? "---" : processor.GetStatValueFormatted(defaultThing, true);
        DefLabel = processor.GetDefLabel();
        if (!IsEmpty)
        {
            Tooltips.Add($"{processor.GetDefLabel()}: {Value}");
        }
    }
}