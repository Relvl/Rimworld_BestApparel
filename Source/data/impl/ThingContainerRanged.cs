using System.Collections.Generic;
using System.Linq;
using BestApparel.compatibility;
using BestApparel.compatibility.stat_processor;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data.impl;

public class ThingContainerRanged : AThingContainer
{
    public override TabId GetTabId() => TabId.Ranged;

    public ThingContainerRanged(ThingDef thingDef) : base(thingDef)
    {
        if (Config.IsCeLoaded)
        {
            CeRangedDamageStatProcessor.TryToLoadAmmo(DefaultThing);
        }
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
            foreach (var processor in CombatExtendedCompat.GetRangedStats(Def)) yield return processor;
        }
        else
        {
            yield return new FuncStatProcessor(thing => thing.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetDamageAmount(thing) ?? 0, "Ranged_Damage");
        }
    }
}