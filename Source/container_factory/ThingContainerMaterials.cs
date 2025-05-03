using Verse;

namespace BestApparel.container_factory;

public class ThingContainerMaterials(ThingDef thingDef, string tabId) : AThingContainer(thingDef, tabId)
{
    public override bool CheckForFilters()
    {
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.thingCategories, nameof(ThingCategoryDef))) return false;
        return true;
    }
}

// ReSharper disable once UnusedType.Global -- reflection: thingTabDef.factoryClass -> DefaultThnigTabRenderer:PrepareData
public class MaterialContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        if (def.IsRangedWeapon) return false;
        if (def.IsMeleeWeapon) return false;
        if (def.destroyOnDrop) return false;
        return def.IsStuff;
    }

    public AThingContainer Produce(ThingDef def, string tabId) => new ThingContainerMaterials(def, tabId);
}