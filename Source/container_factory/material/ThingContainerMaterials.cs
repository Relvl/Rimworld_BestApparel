using Verse;

namespace BestApparel.container_factory;

public class ThingContainerMaterials(ThingDef thingDef, string tabId) : AThingContainer(thingDef, tabId)
{
    public override bool CheckForFilters()
    {
        return BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.thingCategories, nameof(ThingCategoryDef));
    }
}