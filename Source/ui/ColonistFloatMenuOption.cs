using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui;

public class ColonistFloatMenuOption : FloatMenuOption
{
    private static readonly Vector2 PortraitSize = new(36, 36);

    private readonly RenderTexture _texture;
    private readonly float _offset = 27f;

    public ColonistFloatMenuOption(Pawn pawn, Action action) : base(pawn.NameFullColored, action)
    {
        _texture = PortraitsCache.Get(pawn, PortraitSize, Rot4.South, default, 4);
        extraPartWidth = 50;
    }

    public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
    {
        // decompiled and simplified code here
        var fullElementRect = rect;
        --fullElementRect.height;
        var isOver = !Disabled && Mouse.IsOver(fullElementRect);
        Text.Font = GameFont.Small;
        if (tooltip.HasValue) TooltipHandler.TipRegion(rect, tooltip.Value);

        var textureRect = rect;
        textureRect.xMin += 4f;
        textureRect.xMax = rect.x + 27f;
        textureRect.yMin += 4f;
        textureRect.yMax = rect.y + 27f;

        var labelRect = rect;
        labelRect.xMin += 6;
        labelRect.xMax -= 6;
        labelRect.xMax -= 4f;
        labelRect.xMax -= _offset;
        labelRect.x += _offset;

        if (isOver) textureRect.x += 4f;
        if (isOver) labelRect.x += 4f;

        if (!Disabled) MouseoverSounds.DoRegion(fullElementRect);

        var oldColor = GUI.color;
        GUI.color = !Disabled ? !isOver ? ColorBGActive * oldColor : ColorBGActiveMouseover * oldColor : ColorBGDisabled * oldColor;
        GUI.DrawTexture(rect, BaseContent.WhiteTex);
        GUI.color = (!Disabled ? ColorTextActive : ColorTextDisabled) * oldColor;
        Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(labelRect, Label);
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = new Color(iconColor.r, iconColor.g, iconColor.b, iconColor.a * GUI.color.a);
        Widgets.DrawTextureFitted(textureRect, _texture, 1f, new Vector2(1f, 1f), iconTexCoords);

        GUI.color = oldColor;

        if (isOver && mouseoverGuiAction != null)
            mouseoverGuiAction(rect);
        if (tutorTag != null)
            UIHighlighter.HighlightOpportunity(rect, tutorTag);
        if (!Widgets.ButtonInvisible(fullElementRect) || tutorTag != null && !TutorSystem.AllowAction((EventPack)tutorTag))
            return false;
        Chosen(colonistOrdering, floatMenu);
        if (tutorTag != null)
            TutorSystem.Notify_Event((EventPack)tutorTag);
        return true;
    }
}