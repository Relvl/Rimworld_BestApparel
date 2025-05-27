using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace BestApparel.container_factory;

[SuppressMessage("ReSharper", "UnusedType.Global")] // reflection: thingTabDef.factoryClass -> DefaultThingTabRenderer:PrepareData
public class MaterialContainerFactory : IContainerFactory
{
    public bool CanProduce(ThingDef def)
    {
        return def.IsStuff;
    }

    IEnumerable<AThingContainer> IContainerFactory.Produce(ThingDef def, string tabId, bool destructuringStuff)
    {
        yield return new ThingContainerMaterials(def, tabId);
    }
}