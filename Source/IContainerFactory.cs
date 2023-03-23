using Verse;

namespace BestApparel;

public interface IContainerFactory
{
    bool CanProduce(ThingDef def);
    
    AThingContainer Produce(ThingDef def, string tabId);
}