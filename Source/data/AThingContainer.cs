using Verse;

namespace BestApparel.data
{
    public abstract class AThingContainer
    {
        protected ThingDef Def;
        public Thing DefaultThing;

        public abstract void MakeCache();

        public abstract bool CheckForFilters();
    }
}