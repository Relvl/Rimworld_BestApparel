using CombatExtended;
using RimWorld;
using Verse.Sound;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonResetAmmo : AToolbarButton
{
    public ToolbarButtonResetAmmo(ToolbarButtonDef def, IThingTabRenderer renderer) : base(def, renderer)
    {
    }

    public override void Action()
    {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();

        foreach (var container in Renderer.GetAllContainers())
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

        Renderer.UpdateSort();
    }
}