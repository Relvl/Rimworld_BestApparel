using System.Linq;
using CombatExtended;
using UnityEngine;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeAmmoStatProcessor : AStatProcessor
{
    public override float CellWidth => 100;

    public CeAmmoStatProcessor() : base(DefaultStat)
    {
    }

    public override string GetDefName() => "CE_Ammo";
    public override string GetDefLabel() => TranslationCache.StatCEAmmo.Text;
    public override bool IsValueDefault(Thing thing) => false;
    public override float GetStatValue(Thing thing) => 0f;
    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => "";
    public override CellData MakeCell(Thing thing) => new CeAmmoCellData(this, thing);

    public override void RenderCell(Rect cellRect, CellData cell, IThingTabRenderer renderer)
    {
        foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);

        if (cell is CeAmmoCellData cellCe)
        {
            var ammoTypes = cellCe.AmmoUser?.Props?.ammoSet?.ammoTypes;
            var currentAmmoName = cellCe.AmmoUser?.CurrentAmmo.ammoClass.LabelCapShort ?? cellCe.AmmoUser?.CurrentAmmo.ammoClass.LabelCap ?? "---";
            if (ammoTypes is { Count: > 1 })
            {
                cellRect = cellRect.ContractedBy(2);

                if (Widgets.ButtonText(cellRect, currentAmmoName))
                {
                    Find.WindowStack.Add(
                        new FloatMenu(
                            ammoTypes.Select(
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
                                            renderer.UpdateSort();
                                        }
                                    )
                                )
                                .ToList()
                        )
                    );
                }
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(cellRect, currentAmmoName);
            }
        }
    }
}

public class CeAmmoCellData : CellData
{
    public readonly CompAmmoUser AmmoUser;

    public CeAmmoCellData(AStatProcessor processor, Thing thing) : base(processor, thing)
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

    public static AmmoLink GetLink(CompAmmoUser ammoUser)
    {
        if (ammoUser?.Props?.ammoSet != null && AmmoUtility.IsAmmoSystemActive(ammoUser.Props.ammoSet))
        {
            var link = ammoUser.CurrentAmmo.AmmoSetDefs.SelectMany(s => s.ammoTypes).FirstOrDefault(l => l.ammo == ammoUser.CurrentAmmo);
            if (link?.projectile?.projectile != null)
                return link;
        }

        return null;
    }

    public string GetAmmoAndDamage(AmmoLink link) => $"{link.ammo.LabelCap} - {GetDamage(link)} dmg";
}