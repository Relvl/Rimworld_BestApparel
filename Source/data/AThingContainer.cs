using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.data
{
    public abstract class AThingContainer
    {
        protected readonly ThingDef Def;
        public readonly Thing DefaultThing;

        public CellData[] CachedCells { get; protected set; } = { };

        public float CachedSortingWeight { get; protected set; }

        protected AThingContainer(ThingDef thingDef)
        {
            Def = thingDef;
            if (thingDef.MadeFromStuff)
            {
                // todo! деструктуризация по материалу
                // todo! вычислить лучший материал по выбранным параметрам сортировки
                DefaultThing = ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef));
            }
            else
            {
                DefaultThing = ThingMaker.MakeThing(thingDef);
            }
        }

        public abstract void MakeCache();

        public abstract bool CheckForFilters();

        public abstract void MakeSortingWeightsCache();
    }
}