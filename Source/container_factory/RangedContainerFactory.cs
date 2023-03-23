using System.Collections.Generic;
using System.Linq;
using BestApparel.compatibility;
using Verse;

namespace BestApparel.container_factory;

public class ThingContainerRanged : AThingContainer
{
    public ThingContainerRanged(ThingDef thingDef, string tabId) : base(thingDef, tabId)
    {
        if (Config.IsCeLoaded)
        {
            CombatExtendedCompat.TryToLoadAmmo(DefaultThing);
        }
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.thingCategories)) return false;
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.weaponClasses)) return false;
        return true;
    }

    public IEnumerable<AStatProcessor> CollectStats()
    {
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

// ReSharper disable once UnusedType.Global -- reflection: thingTabDef.factoryClass -> DefaultThnigTabRenderer:PrepareData
public class RangedContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        if (def.destroyOnDrop) return false;
        if (!def.IsRangedWeapon) return false;
        return true;
    }

    public AThingContainer Produce(ThingDef def, string tabId)
    {
        return new ThingContainerRanged(def, tabId);
    }
}