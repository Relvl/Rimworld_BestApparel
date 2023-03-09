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

        public CellData(AStatProcessor processor, Thing defaultThing)
        {
            DefName = processor.GetStatDef().defName;
            ValueRaw = processor.GetStatValue(defaultThing);
            IsEmpty = processor.IsValueDefault(defaultThing);
            Value = IsEmpty ? "---" : processor.GetStatValueFormatted(defaultThing, true);
            WeightFactor = BestApparel.Config.Sorting.Apparel[DefName] + Config.MaxSortingWeight;
            DefLabel = processor.GetStatDef().label;
            if (!IsEmpty)
            {
                Tooltips.Add($"{processor.GetStatDef().label}: {Value}");
            }
        }
    }
}