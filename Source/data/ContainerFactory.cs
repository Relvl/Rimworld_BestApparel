using System;
using BestApparel.data.impl;
using Verse;

namespace BestApparel.data;

public class ContainerFactory
{
    public AThingContainer Produce(ThingDef thingDef)
    {
        try
        {
            if (thingDef.destroyOnDrop) return null;

            if (thingDef.IsApparel)
            {
                return new ThingContainerApparel(thingDef);
            }

            if (thingDef.IsRangedWeapon)
            {
                return new ThingContainerRanged(thingDef);
            }

            if (thingDef.IsMeleeWeapon && !thingDef.IsStuff && !thingDef.IsIngestible && !thingDef.IsDrug)
            {
                return new ThingContainerMelee(thingDef);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Can not produce container for ThingDef '{thingDef.defName}' -> {e.Message}");
        }

        return null;
    }
}