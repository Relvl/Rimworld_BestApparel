using System;
using System.Globalization;
using Verse;

namespace BestApparel.stat_processor
{
    public class FuncStatProcessor : AStatProcessor
    {
        private readonly Func<Thing, float> _func;
        private readonly string _name;

        public FuncStatProcessor(Func<Thing, float> func, string name) : base(DEFAULT)
        {
            _func = func;
            _name = name;
        }

        public override string GetDefName()
        {
            return _name;
        }

        public override string GetDefLabel()
        {
            return _name.Translate();
        }

        public override float GetStatValue(Thing thing)
        {
            return _func(thing);
        }

        public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
        {
            return GetStatValue(thing).ToString(CultureInfo.CurrentCulture);
        }
    }
}