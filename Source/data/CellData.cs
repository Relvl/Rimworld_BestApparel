namespace BestApparel.data
{
    public class CellData
    {
        public readonly string DefName;
        public readonly string Value;
        public readonly string[] Tooltips;
        public readonly bool None;

        public CellData(string defName, string value, string[] tooltips, bool none)
        {
            DefName = defName;
            Value = value;
            Tooltips = tooltips;
            None = none;
        }
    }
}