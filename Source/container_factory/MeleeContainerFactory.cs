using System.Collections.Generic;
using BestApparel.compatibility;
using BestApparel.stat_collector;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.container_factory;

public class ThingContainerMelee : AThingContainer
{
    public ThingContainerMelee(ThingDef thingDef, string tabId) : base(thingDef, tabId)
    {
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
            yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
            yield return new CommonStatProcessor(StatDef.Named("MeleeCounterParryBonus"));
            foreach (var processor in CombatExtendedCompat.GetMeleeStats(Def)) yield return processor;
        }
    }
}

// ReSharper disable once UnusedType.Global -- reflection: thingTabDef.factoryClass -> DefaultThnigTabRenderer:PrepareData
public class MeleeContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        if (def.destroyOnDrop) return false;
        if (!def.IsMeleeWeapon) return false;
        if (def.IsStuff) return false;
        if (def.IsIngestible) return false;
        if (def.IsDrug) return false;
        return true;
    }

    public AThingContainer Produce(ThingDef def, string tabId) => new ThingContainerMelee(def, tabId);
}