using System.Collections.Generic;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data.impl;

public class ThingContainerApparel : AThingContainer
{
    public override TabId GetTabId() => TabId.Apparel;

    public ThingContainerApparel(ThingDef thingDef) : base(thingDef)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.Apparel.Layer.IsCollectionAllowed(Def.apparel.layers)) return false;
        if (!BestApparel.Config.Apparel.BodyPart.IsCollectionAllowed(Def.apparel.bodyPartGroups)) return false;
        if (!BestApparel.Config.Apparel.Category.IsCollectionAllowed(Def.thingCategories)) return false;
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

        if (Def.equippedStatOffsets != null)
        {
            foreach (var statOffset in Def.equippedStatOffsets)
            {
                var stat = statOffset?.stat;
                if (stat == null) continue;
                var proc = new CommonStatProcessor(stat);
                if (!proc.IsValueDefault(DefaultThing))
                {
                    yield return proc;
                }
            }
        }
    }
}