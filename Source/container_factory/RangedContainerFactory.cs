using System.Collections.Generic;
using System.Linq;
using BestApparel.compatibility;
using BestApparel.stat_processor;
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
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.weaponClasses, nameof(WeaponClassDef))) return false;
        return true;
    }

    public IEnumerable<AStatProcessor> CollectStats()
    {
        if (Config.IsCeLoaded)
        {
            foreach (var processor in CombatExtendedCompat.GetRangedStats(Def)) yield return processor;
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

    public AThingContainer Produce(ThingDef def, string tabId) => new ThingContainerRanged(def, tabId);
}