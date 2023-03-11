using System.Collections.Generic;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data.impl;

public class ThingContainerMelee : AThingContainer
{
    public override TabId GetTabId() => TabId.Melee;

    public ThingContainerMelee(ThingDef thingDef) : base(thingDef)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.Melee.Category.IsCollectionAllowed(Def.thingCategories)) return false;
        return true;
    }

    public override IEnumerable<AStatProcessor> CollectStats()
    {
        foreach (var def in DefDatabase<StatDef>.AllDefs)
        {
            if (def.Worker.ShouldShowFor(StatRequest.For(DefaultThing)) && !def.Worker.IsDisabledFor(DefaultThing))
            {
                var proc = new BaseStatProcessor(def);
                if (!proc.IsValueDefault(DefaultThing))
                {
                    yield return proc;
                }
            }
        }

        if (Config.IsCeLoaded)
        {
            yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
            yield return new CommonStatProcessor(StatDef.Named("MeleeCounterParryBonus"));
        }
    }
}