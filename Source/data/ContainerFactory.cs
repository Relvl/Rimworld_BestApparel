using System;
using Verse;

namespace BestApparel.data;

public class ContainerFactory
{
    public AThingContainer Produce(ThingDef thingDef)
    {
        try
        {
            if (thingDef.IsApparel)
            {
                return new ThingContainerApparel(thingDef);
            }

            if (thingDef.IsWeapon && thingDef.weaponTags != null && thingDef.weaponTags.Contains("Gun"))
            {
                return new ThingContainerRanged(thingDef);
            }
        }
        catch (Exception e)
        {
            Log.Warning($"Can not procuce container for ThingDef '{thingDef.defName}' -> NRE {e.Message}");
        }

        return null;
    }
}