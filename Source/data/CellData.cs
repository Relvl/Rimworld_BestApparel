using System.Collections.Generic;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.data;

public class CellData
{
    public readonly List<string> Tooltips = new();

    public AStatProcessor Processor;
    public Thing Thing;
    public bool IsEmpty = true;
    public string DefLabel = "";
    public float ValueRaw;
    public string Value = "---";
    public float WeightFactor = 1;
    public float NormalizedWeight = 0;

    protected CellData()
    {
    }

    public CellData(AStatProcessor processor, Thing thing)
    {
        Processor = processor;
        Thing = thing;
        IsEmpty = processor.IsValueDefault(thing);
        ValueRaw = processor.GetStatValue(thing);
        Value = IsEmpty ? "---" : processor.GetStatValueFormatted(thing, true);
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