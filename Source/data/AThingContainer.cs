using Verse;

namespace BestApparel.data
{
    public abstract class AThingContainer
    {
        public ThingDef Def;
        public Thing DefaultThing;

        public abstract void MakeCache();
    }
}