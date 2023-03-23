using Verse;

namespace BestApparel.container_factory;

public class ThingContainerApparel : AThingContainer
{
    public ThingContainerApparel(ThingDef thingDef, string tabId) : base(thingDef, tabId)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.apparel.layers, nameof(ApparelLayerDef))) return false;
        if (!BestApparel.Config.CheckFilter(TabIdStr, Def.apparel.bodyPartGroups, nameof(BodyPartGroupDef))) return false;
        return true;
    }
}

// ReSharper disable once UnusedType.Global -- reflection: thingTabDef.factoryClass -> DefaultThnigTabRenderer:PrepareData
public class ApparelContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        if (def.destroyOnDrop) return false;
        if (!def.IsApparel) return false;
        return true;
    }

    public AThingContainer Produce(ThingDef def, string tabId) => new ThingContainerApparel(def, tabId);
}