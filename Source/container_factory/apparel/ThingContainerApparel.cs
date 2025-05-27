using Verse;

namespace BestApparel.container_factory;

public class ThingContainerApparel(ThingDef thingDef, string tabId) : AThingContainer(thingDef, tabId)
{
    public override bool CheckForFilters()
    {
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.apparel.layers, nameof(ApparelLayerDef))) return false;
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.apparel.bodyPartGroups, nameof(BodyPartGroupDef))) return false;
        return true;
    }
}