using System.Linq;
using CombatExtended;
using UnityEngine;
using Verse;

namespace BestApparel.compatibility.stat_processor;

public class CeRangedDamageStatProcessor : AStatProcessor
{
    public override float CellWidth => 120;

    public CeRangedDamageStatProcessor() : base(DefaultStat)
    {
    }

    public override string GetDefName() => "Ranged_Damage";

    public override string GetDefLabel() => "Ranged_Damage";

    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0;

    public override float GetStatValue(Thing thing)
    {
        var ammoUser = thing.TryGetComp<CompAmmoUser>();
        var link = CellDataCeRangedDamage.GetLink(ammoUser);
        if (link is null) return thing.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetDamageAmount(thing) ?? -1;
        float damageLabel = link.projectile.projectile.GetDamageAmount(thing);

        if (link.projectile.projectile is ProjectilePropertiesCE projProps)
        {
            if (!projProps.secondaryDamage.NullOrEmpty())
            {
                damageLabel += projProps.secondaryDamage.Sum(secondaryDamage => secondaryDamage.amount * secondaryDamage.chance);
            }

            if (projProps.pelletCount > 1)
            {
                damageLabel *= projProps.pelletCount;
            }
        }

        return damageLabel;
    }

    public override CellData MakeCell(Thing thing) => new CellDataCeRangedDamage(this, thing);

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => GetStatValue(thing).ToStringByStyle(ToStringStyle.Integer);

    public override void RenderCell(Rect cellRect, CellData cell)
    {
        Widgets.Label(cellRect, cell.Value);
        foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);

        if (cell is CellDataCeRangedDamage cellCe && cellCe.AmmoUser?.Props?.ammoSet?.ammoTypes is { Count: > 1 })
        {
            cellRect.xMin += 34;
            cellRect = cellRect.ContractedBy(2);

            if (Widgets.ButtonText(cellRect, cellCe.AmmoUser.CurrentAmmo.ammoClass.LabelCapShort ?? cellCe.AmmoUser.CurrentAmmo.ammoClass.LabelCap))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        cellCe.AmmoUser.Props.ammoSet.ammoTypes //
                            .Select(
                                ammoLink => new FloatMenuOption(
                                    cellCe.GetAmmoAndDamage(ammoLink),
                                    () =>
                                    {
                                        var defaultProjectile = cell.Thing.def.Verbs.FirstOrDefault(it => it is VerbPropertiesCE)?.defaultProjectile;
                                        if (ammoLink.projectile.defName == defaultProjectile?.defName)
                                            BestApparel.Config.RangedAmmo.Remove(cell.Thing.def.defName);
                                        else
                                            BestApparel.Config.RangedAmmo[cell.Thing.def.defName] = ammoLink.projectile.defName;

                                        cellCe.AmmoUser.CurrentAmmo = ammoLink.ammo;
                                        // window.DataProcessor.Rebuild();
                                    }
                                )
                            )
                            .ToList()
                    )
                );
            }
        }
    }

    public static void TryToLoadAmmo(Thing thing)
    {
        if (!BestApparel.Config.RangedAmmo.ContainsKey(thing.def.defName)) return;
        var ammoDefToLoad = BestApparel.Config.RangedAmmo[thing.def.defName];
        if (ammoDefToLoad.NullOrEmpty()) return;
        var ammoUser = thing.TryGetComp<CompAmmoUser>();
        var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
        if (link is null) return;
        ammoUser.CurrentAmmo = link.ammo;
    }

    // public static void OnRangedRestoreAmmoClick(DataProcessor processor)
    // {
    //     SoundDefOf.Tick_High.PlayOneShotOnCamera();
    //     // foreach (var container in processor.GetContainers(TabId.Ranged))
    //     // {
    //     //     var thing = container.DefaultThing;
    //     //     if (!BestApparel.Config.RangedAmmo.ContainsKey(thing.def.defName)) continue;
    //     //     BestApparel.Config.RangedAmmo.Remove(thing.def.defName);
    //     //     var ammoDefToLoad = thing.def.Verbs?.FirstOrDefault(it => it is VerbPropertiesCE)?.defaultProjectile?.defName;
    //     //     if (ammoDefToLoad.NullOrEmpty()) continue;
    //     //     var ammoUser = thing.TryGetComp<CompAmmoUser>();
    //     //     var link = ammoUser?.Props.ammoSet.ammoTypes.FirstOrDefault(l => l.projectile.defName == ammoDefToLoad);
    //     //     if (link is null) continue;
    //     //     ammoUser.CurrentAmmo = link.ammo;
    //     // }
    // }
}

internal class CellDataCeRangedDamage : CellData
{
    public readonly CompAmmoUser AmmoUser;

    public CellDataCeRangedDamage(AStatProcessor processor, Thing thing)
    {
        Processor = processor;
        Thing = thing;
        DefLabel = processor.GetDefLabel();
        ValueRaw = thing.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetDamageAmount(thing) ?? 0;

        AmmoUser = thing.TryGetComp<CompAmmoUser>();
        var link = GetLink(AmmoUser);
        if (link != null)
        {
            ValueRaw = GetDamage(link);
            Tooltips.Add(link.projectile.GetProjectileReadout(thing));
            Tooltips.Add($"Caliber: {AmmoUser.Props.ammoSet.LabelCap}");
        }

        Value = ValueRaw.ToStringByStyle(ToStringStyle.Integer);
        IsEmpty = ValueRaw != 0;
    }

    private float GetDamage(AmmoLink link)
    {
        if (link == null) return 0;
        float damageLabel = link.projectile.projectile.GetDamageAmount(Thing);

        if (link.projectile.projectile is ProjectilePropertiesCE projProps)
        {
            if (!projProps.secondaryDamage.NullOrEmpty())
            {
                damageLabel += projProps.secondaryDamage.Sum(secondaryDamage => secondaryDamage.amount * secondaryDamage.chance);
            }

            if (projProps.pelletCount > 1)
            {
                damageLabel *= projProps.pelletCount;
            }
        }

        return damageLabel;
    }

    public string GetAmmoAndDamage(AmmoLink link) => $"{link.ammo.LabelCap} - {GetDamage(link)} dmg";

    public static AmmoLink GetLink(CompAmmoUser ammoUser)
    {
        if (ammoUser?.Props?.ammoSet != null && AmmoUtility.IsAmmoSystemActive(ammoUser.Props.ammoSet))
        {
            var link = ammoUser.CurrentAmmo.AmmoSetDefs.SelectMany(s => s.ammoTypes).FirstOrDefault(l => l.ammo == ammoUser.CurrentAmmo);
            if (link?.projectile?.projectile != null)
            {
                return link;
            }
        }

        return null;
    }
}