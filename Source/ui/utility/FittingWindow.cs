using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.data.impl;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class FittingWindow : Window
{
    public override Vector2 InitialSize => new(650, 500);

    private readonly MainTabWindow _parent;
    private Vector2 _scrollLeft = Vector2.zero;
    private Vector2 _scrollRight = Vector2.zero;
    private BodyPartGroupDef _selectedBodyPart;

    private List<ThingContainerApparel> _worn = new();
    private float _lastFrameListHeight;

    public FittingWindow(MainTabWindow parent)
    {
        resizeable = true;
        _parent = parent;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var bodyParts = _parent.DataProcessor.GetApparelBodyParts();
        var allApparels = _parent.DataProcessor.GetAllApparels();

        // нарисовать все бодипарты и назначенные на них шмотки.
        // нарисовать все шмотки, исключая назначенные
        // *** нарисовать кнопку выбора превью колониста?
        // галочка, включающая сортировку по выбранным статам и настройкам сортировки

        // label 
        Text.Font = GameFont.Medium;
        GUI.color = Color.green;
        Widgets.Label(inRect, "BestApparel.WindowTitle.Fitting".Translate());
        inRect.yMin += 46;

        const int padding = 10;
        const int cellPadding = 3;
        const int bpCellHeight = 20;
        const int apCellHeight = 24 + cellPadding * 2;
        const int iconSize = 24;

        // =================================================

        var paneRect = new Rect(0, inRect.yMin, inRect.width / 2 - padding, inRect.height - padding - /*btnHeight*/ 24);
        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var cellRightOffset = _lastFrameListHeight > paneRect.height ? 16 : 0;
        var scrollInnerRect = new Rect(0, 0, paneRect.width - cellPadding * 2 - cellRightOffset, _lastFrameListHeight);

        Widgets.BeginScrollView(paneRect, ref _scrollLeft, scrollInnerRect);
        var cellRect = new Rect(cellPadding, cellPadding, scrollInnerRect.width, bpCellHeight);
        for (var bpIdx = 0; bpIdx < bodyParts.Count; bpIdx++)
        {
            var bodyPart = bodyParts[bpIdx];
            GUI.color = Color.white;
            Text.Font = GameFont.Small;

            var apparels = GetApparelsForBodyPart(bodyPart);

            cellRect.height = bpCellHeight + (apparels.Length > 0 ? apparels.Length * 26 : 18);

            if (Widgets.ButtonInvisible(cellRect)) _selectedBodyPart = _selectedBodyPart == null ? bodyPart : null;

            GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (Mouse.IsOver(cellRect)) GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (_selectedBodyPart == bodyPart) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

            // body part name
            Text.Font = GameFont.Tiny;
            // todo! font if have any or not
            Widgets.Label(new Rect(cellRect.x + cellPadding, cellRect.y + cellPadding, cellRect.width, 16), bodyPart.label);

            // todo btn clear slot


            // apparels in slot name
            Text.Font = GameFont.Small;
            GUI.color = BestApparel.ColorWhiteA50;


            if (apparels.Any())
            {
                for (var thingIdx = 0; thingIdx < apparels.Length; thingIdx++)
                {
                    var apparel = apparels[thingIdx];
                    var offset = 16 + thingIdx * 26;
                    Widgets.ThingIcon(new Rect(cellRect.x + cellPadding, cellRect.y + cellPadding + offset, iconSize, iconSize), apparel.DefaultThing);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(new Rect(cellRect.x + iconSize + cellPadding * 2, cellRect.y + cellPadding + offset, scrollInnerRect.width, 24), apparel.Def.label);
                    Text.Anchor = TextAnchor.UpperLeft;

                    // todo! tooltip: thing tooltip
                }
            }
            else
            {
                Widgets.Label(new Rect(cellRect.x + cellPadding, cellRect.y + cellPadding + 16, scrollInnerRect.width, 24), "BestAparel.Label.Fitting.SelectApparel".Translate());
            }

            // todo remove apparel

            cellRect.y += cellRect.height + cellPadding;
        }

        _lastFrameListHeight = cellRect.y + cellRect.height;

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Widgets.EndScrollView();
        paneRect = paneRect.ExpandedBy(1);

        // =================================================

        // todo как это кешировать? надо бы пересчитывать только при изменениях в... чем-либо
        var filtered = allApparels.Where(CanWear).ToList();

        paneRect.x = inRect.width / 2 + padding;
        Widgets.DrawMenuSection(paneRect);
        paneRect = paneRect.ContractedBy(1);

        var scrolledHeight = (apCellHeight + cellPadding) * filtered.Count + cellPadding * 2;
        cellRightOffset = scrolledHeight > paneRect.height ? 16 : 0;
        scrollInnerRect = new Rect(0, 0, paneRect.width - cellPadding * 2 - cellRightOffset, scrolledHeight);

        Widgets.BeginScrollView(paneRect, ref _scrollRight, scrollInnerRect);
        cellRect = new Rect(cellPadding, cellPadding, scrollInnerRect.width, apCellHeight);
        foreach (var apparel in filtered)
        {
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            GUI.DrawTexture(cellRect, TexUI.HighlightTex);
            if (Mouse.IsOver(cellRect)) GUI.DrawTexture(cellRect, TexUI.HighlightTex);

            // Thing icon
            Widgets.ThingIcon(new Rect(cellRect.x + cellPadding, cellRect.y + cellPadding, iconSize, iconSize), apparel.DefaultThing);

            // Button [+]
            var btnRect = new Rect(cellRect.x + cellRect.width - cellPadding * 2 - iconSize, cellRect.y + cellPadding, iconSize, iconSize);
            if (Widgets.ButtonText(btnRect, "+")) AddWorn(apparel);

            TooltipHandler.TipRegion(btnRect, "BestApparel.Button.Fitting.AddToSlot".Translate());

            // Body part name
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleLeft;
            var labelRect = new Rect(
                cellRect.x + iconSize + cellPadding * 2,
                cellRect.y + cellPadding,
                cellRect.width - cellPadding * 3 - iconSize * 2,
                cellRect.height - cellPadding * 2
            );
            Widgets.Label(labelRect, apparel.Def.label);
            var tip = apparel.DefaultThing.LabelNoParenthesisCap.AsTipTitle() + //
                      GenLabel.LabelExtras(apparel.DefaultThing, 1, true, true) +
                      "\n\n" +
                      apparel.DefaultThing.DescriptionDetailed;
            TooltipHandler.TipRegion(cellRect, tip);

            cellRect.y += cellRect.height + cellPadding;
        }

        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.EndScrollView();
    }

    private ThingContainerApparel[] GetApparelsForBodyPart(BodyPartGroupDef bodyPart) => _worn.Where(it => it.Def.apparel.bodyPartGroups.Contains(bodyPart)).ToArray();

    private void AddWorn(ThingContainerApparel apparel)
    {
        _worn.RemoveIf(containerApparel => !ApparelUtility.CanWearTogether(containerApparel.Def, apparel.Def, BodyDefOf.Human));
        _worn.Add(apparel);
        _selectedBodyPart = null;
    }

    private bool CanWear(ThingContainerApparel check)
    {
        if (_selectedBodyPart != null)
        {
            return check.Def.apparel.bodyPartGroups.Contains(_selectedBodyPart);
        }

        return _worn.All(worn => ApparelUtility.CanWearTogether(worn.Def, check.Def, BodyDefOf.Human));
    }
}