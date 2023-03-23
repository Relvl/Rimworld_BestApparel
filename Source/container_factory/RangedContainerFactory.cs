using Verse;

namespace BestApparel.container_factory;

public class ThingContainerRanged : AThingContainer
{
    public ThingContainerRanged(ThingDef thingDef, string tabId) : base(thingDef, tabId)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.weaponClasses, nameof(WeaponClassDef))) return false;
        return true;
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