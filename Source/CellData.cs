using System.Collections.Generic;
using Verse;

namespace BestApparel;

public class CellData
{
    public readonly List<string> Tooltips = [];
    public bool IsEmpty;
    public float NormalizedWeight = 0;

    public AStatProcessor Processor;
    public Thing Thing;
    public string Value;
    public float WeightFactor = 1;

    public CellData(AStatProcessor processor, Thing thing)
    {
        Processor = processor;
        Thing = thing;
        IsEmpty = processor.IsValueDefault(thing);
        var valueRaw = processor.GetStatValue(thing);
        Value = processor.GetStatValueFormatted(thing);

        if (Prefs.DevMode) Tooltips.Add($"Raw value: {valueRaw}, Default: {processor.StatDef.defaultBaseValue}");
    }
}