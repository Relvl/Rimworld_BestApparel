using RimWorld;
using Verse;

namespace BestApparel.data
{
    public class ApparelThing : ComparableThing
    {
        public ApparelThing(ThingDef thingDef)
        {
            Def = thingDef;

            // todo! деструктуризация по материалу
            // todo! вычислить лучший материал по выбранным параметрам сортировки

            DefaultThing = thingDef.MadeFromStuff
                ? ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef))
                : ThingMaker.MakeThing(thingDef);
        }
    }
}