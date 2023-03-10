using System.Collections.Generic;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.data
{
    public class CellData
    {
        public readonly string DefName;
        public readonly float ValueRaw;
        public readonly string Value;
        public readonly bool IsEmpty;
        public readonly float WeightFactor;
        public readonly string DefLabel;
        public readonly List<string> Tooltips = new List<string>();

        public float NormalizedWeight { get; set; }

        public CellData(AStatProcessor processor, Thing defaultThing, float weightFactor)
        {
            WeightFactor = weightFactor;
            DefName = processor.GetDefName();
            ValueRaw = processor.GetStatValue(defaultThing);
            IsEmpty = processor.IsValueDefault(defaultThing);
            Value = IsEmpty ? "---" : processor.GetStatValueFormatted(defaultThing, true);
            DefLabel = processor.GetDefLabel();
            if (!IsEmpty)
            {
                Tooltips.Add($"{processor.GetDefLabel()}: {Value}");
            }
        }
    }
}