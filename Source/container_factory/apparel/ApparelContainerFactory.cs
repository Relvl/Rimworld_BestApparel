using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace BestApparel.container_factory;

[SuppressMessage("ReSharper", "UnusedType.Global")] // reflection: thingTabDef.factoryClass -> DefaultThingTabRenderer:PrepareData
public class ApparelContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        return !def.destroyOnDrop && !def.IsStuff && def.IsApparel;
    }

    IEnumerable<AThingContainer> IContainerFactory.Produce(ThingDef def, string tabId, bool destructuringStuff)
    {
        yield return new ThingContainerApparel(def, tabId);
    }
}