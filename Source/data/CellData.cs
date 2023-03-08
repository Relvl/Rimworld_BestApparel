using RimWorld;

namespace BestApparel.data
{
    public class CellData
    {
        public readonly string DefName;
        public readonly string Value;
        public readonly float ValueRaw;
        public readonly string[] Tooltips;
        public readonly bool None;
        public readonly StatDef StatDef;

        public float NormalizedWeight = 0f;
        public float WeightFactor = Config.MaxSortingWeight;

        public CellData(string defName, string value, string[] tooltips, bool none, float valueRaw, StatDef statDef)
        {
            DefName = defName;
            Value = value;
            Tooltips = tooltips;
            None = none;
            ValueRaw = valueRaw;
            StatDef = statDef;
        }
    }
}