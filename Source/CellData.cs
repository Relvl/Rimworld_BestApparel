using System.Collections.Generic;
using Verse;

namespace BestApparel;

public class CellData
{
    public readonly List<string> Tooltips = new();

    public AStatProcessor Processor;
    public Thing Thing;
    public bool IsEmpty;
    public string Value;
    public float WeightFactor = 1;
    public float NormalizedWeight = 0;

    public CellData(AStatProcessor processor, Thing thing)
    {
        Processor = processor;
        Thing = thing;
        IsEmpty = processor.IsValueDefault(thing);
        var valueRaw = processor.GetStatValue(thing);
        Value = processor.GetStatValueFormatted(thing);

        if (Prefs.DevMode)
        {
            Tooltips.Add($"Raw value: {valueRaw}, Default: {processor.GetStatDef().defaultBaseValue}");
        }
    }
}