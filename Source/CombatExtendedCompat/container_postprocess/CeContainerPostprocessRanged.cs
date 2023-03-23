using CombatExtended;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once UnusedType.Global -- reflective: DefaultThnigTabRenderer:ctor() -> ContainerPostprocessDef
public class CeContainerPostprocessRanged : IContainerPostprocess
{
    public void Postprocess(AThingContainer container, IThingTabRenderer renderer)
    {
        if (!BestApparel.Config.RangedAmmo.ContainsKey(container.Def.defName)) return;
        var ammoDefToLoad = BestApparel.Config.RangedAmmo[container.Def.defName];
        if (ammoDefToLoad.NullOrEmpty()) return;
        var ammoUser = container.DefaultThing.TryGetComp<CompAmmoUser>();
        var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
        if (link is null) return;
        ammoUser.CurrentAmmo = link.ammo;
    }
}