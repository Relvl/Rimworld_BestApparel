using Verse;

namespace BestApparel.container_factory;

public class ThingContainerRanged(ThingDef thingDef, string tabId) : AThingContainer(thingDef, tabId)
{
    public override bool CheckForFilters()
    {
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.weaponClasses, nameof(WeaponClassDef))) return false;
        return true;
    }
}

// ReSharper disable once UnusedType.Global -- reflection: thingTabDef.factoryClass -> DefaultThnigTabRenderer:PrepareData
public class RangedContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        if (def.destroyOnDrop) return false;
        if (def.IsStuff) return false;
        if (!def.IsRangedWeapon) return false;
        return true;
    }

    public AThingContainer Produce(ThingDef def, string tabId) => new ThingContainerRanged(def, tabId);
}