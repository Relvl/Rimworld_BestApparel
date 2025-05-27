using Verse;

namespace BestApparel.container_factory;

public class ThingContainerMelee(ThingDef thingDef, string tabId) : AThingContainer(thingDef, tabId)
{
    public override bool CheckForFilters()
    {
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.thingCategories, nameof(ThingCategoryDef))) return false;
        if (!BestApparel.GetTabConfig(TabIdStr).Filters.CheckFilter(Def.weaponClasses, nameof(WeaponClassDef))) return false;
        return true;
    }
}