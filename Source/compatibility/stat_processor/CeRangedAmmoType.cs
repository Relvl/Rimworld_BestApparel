using System.Linq;
using BestApparel.data;
using BestApparel.stat_processor;
using CombatExtended;
using RimWorld;
using UnityEngine;
using Verse;
using MainTabWindow = BestApparel.ui.MainTabWindow;

namespace BestApparel.compatibility.stat_processor;

public class CeRangedAmmoType : AStatProcessor
{
    public override float CellWidth => 120;

    public CeRangedAmmoType() : base(DefaultStat)
    {
    }

    public override string GetDefName() => "Ranged_Damage";

    public override string GetDefLabel() => "Ranged_Damage";

    public override bool IsValueDefault(Thing thing) => GetStatValue(thing) == 0;

    public override float GetStatValue(Thing thing)
    {
        var CompAmmoUser = thing.TryGetComp<CompAmmoUser>();
        if (CompAmmoUser is null || !AmmoUtility.IsAmmoSystemActive(CompAmmoUser.Props.ammoSet)) return 0;
        var link = CompAmmoUser.CurrentAmmo.AmmoSetDefs.SelectMany(s => s.ammoTypes).FirstOrDefault(l => l.ammo == CompAmmoUser.CurrentAmmo);
        if (link is null) return 0;
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

    public override void RenderCell(Rect cellRect, CellData cell, MainTabWindow window)
    {
        Widgets.Label(cellRect, cell.Value);
        foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);

        if (cell is CellDataCeRangedDamage cellCe)
        {
            cellRect.xMin += 38;

            if (Widgets.ButtonText(cellRect, cellCe.CompAmmoUser.CurrentAmmo.ammoClass.LabelCapShort ?? cellCe.CompAmmoUser.CurrentAmmo.ammoClass.LabelCap))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        cellCe.CompAmmoUser.Props.ammoSet.ammoTypes //
                            .Select(
                                ammoLink => new FloatMenuOption(
                                    cellCe.GetAmmoAndDamage(ammoLink),
                                    () =>
                                    {
                                        cellCe.CompAmmoUser.CurrentAmmo = ammoLink.ammo;
                                        window.DataProcessor.Rebuild();
                                    }
                                )
                            )
                            .ToList()
                    )
                );
            }
        }
    }
}

public class CellDataCeRangedDamage : CellData
{
    public readonly CompAmmoUser CompAmmoUser;

    public CellDataCeRangedDamage(AStatProcessor processor, Thing thing)
    {
        Processor = processor;
        Thing = thing;
        DefLabel = processor.GetDefLabel();

        CompAmmoUser = thing.TryGetComp<CompAmmoUser>();
        if (CompAmmoUser == null || !AmmoUtility.IsAmmoSystemActive(CompAmmoUser.Props.ammoSet)) return;
        var link = CompAmmoUser.CurrentAmmo.AmmoSetDefs.SelectMany(s => s.ammoTypes).FirstOrDefault(l => l.ammo == CompAmmoUser.CurrentAmmo);
        if (link == null) return;

        ValueRaw = GetDamage(link);
        Value = ValueRaw.ToStringByStyle(ToStringStyle.Integer);
        IsEmpty = ValueRaw != 0;

        Tooltips.Add(link.projectile.GetProjectileReadout(thing));
        Tooltips.Add($"Caliber: {CompAmmoUser.Props.ammoSet.LabelCap}");
    }

    public float GetDamage(AmmoLink link)
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
}