using System.Collections.Generic;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data;

public class ThingContainerRanged : AThingContainer
{
    public override TabId GetTabId() => TabId.Ranged;

    public ThingContainerRanged(ThingDef thingDef) : base(thingDef)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.Ranged.Category.IsCollectionAllowed(Def.thingCategories)) return false;
        if (!BestApparel.Config.Ranged.WeaponClass.IsCollectionAllowed(Def.weaponClasses)) return false;
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
            foreach (var verb in Def.Verbs)
            {
                if (verb.verbClass.FullName == "CombatExtended.Verb_ShootCE")
                {
                    yield return new FuncStatProcessor(_ => verb.range, "Ability_Range.label");
                }
            }

            yield return new CommonStatProcessor(StatDef.Named("MagazineCapacity"));
        }
    }
}