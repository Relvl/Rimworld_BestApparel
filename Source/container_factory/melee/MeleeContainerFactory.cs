using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace BestApparel.container_factory;

[SuppressMessage("ReSharper", "UnusedType.Global")] // reflection: thingTabDef.factoryClass -> DefaultThingTabRenderer:PrepareData
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

    IEnumerable<AThingContainer> IContainerFactory.Produce(ThingDef def, string tabId, bool destructuringStuff)
    {
        yield return new ThingContainerMelee(def, tabId);
    }
}