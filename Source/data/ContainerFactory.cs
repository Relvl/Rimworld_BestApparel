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
            if (thingDef.IsApparel)
            {
                return new ThingContainerApparel(thingDef);
            }

            // todo thingCategories["Grenades"] 
            if ( /*thingDef.IsRangedWeapon ||*/ (thingDef.IsWeapon && thingDef.weaponTags != null && thingDef.weaponTags.Contains("Gun")))
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
            Log.Warning($"Can not procuce container for ThingDef '{thingDef.defName}' -> NRE {e.Message}");
        }

        return null;
    }
}