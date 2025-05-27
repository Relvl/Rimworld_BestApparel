using System.Collections.Generic;
using Verse;

namespace BestApparel;

public interface IContainerFactory
{
    bool CanProduce(ThingDef def);

    IEnumerable<AThingContainer> Produce(ThingDef def, string tabId, bool destructuringStuff);
}