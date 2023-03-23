using System;
using System.Collections.Generic;
using BestApparel.def;
using BestApparel.thing_tab_renderer;
using CombatExtended;
using RimWorld;
using Verse;
using Verse.Sound;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once ClassNeverInstantiated.Global, UnusedType.Global -- instantiated by reflection: ThingTabDef.renderClass -> ThingTab:ctor
public class CeRangedThingTabRenderer : DefaultThnigTabRenderer
{
    public CeRangedThingTabRenderer(ThingTabDef def) : base(def)
    {
    }

    public override IEnumerable<(TranslationCache.E, Action)> GetToolbarRight()
    {
        yield return (TranslationCache.BtnRangedRestoreAmmo, OnRangedRestoreAmmoClick);
    }

    public override void PostProcessContainer(AThingContainer container)
    {
        if (!BestApparel.Config.RangedAmmo.ContainsKey(container.Def.defName)) return;
        var ammoDefToLoad = BestApparel.Config.RangedAmmo[container.Def.defName];
        if (ammoDefToLoad.NullOrEmpty()) return;
        var ammoUser = container.DefaultThing.TryGetComp<CompAmmoUser>();
        var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
        if (link is null) return;
        ammoUser.CurrentAmmo = link.ammo;
    }

    private void OnRangedRestoreAmmoClick()
    {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        foreach (var container in AllContainers)
        {
            if (!BestApparel.Config.RangedAmmo.ContainsKey(container.Def.defName)) continue;

            var ammoDefToLoad = container.Def.Verbs?.FirstOrDefault(it => it is VerbPropertiesCE)?.defaultProjectile?.defName;
            if (ammoDefToLoad.NullOrEmpty()) continue;
            var ammoUser = container.DefaultThing.TryGetComp<CompAmmoUser>();
            var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
            if (link is null) continue;
            ammoUser.CurrentAmmo = link.ammo;

            BestApparel.Config.RangedAmmo.Remove(container.Def.defName);
        }

        UpdateSort();
    }
}