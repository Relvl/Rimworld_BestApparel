using BestApparel.def;
using UnityEngine;
using Verse;

namespace BestApparel;

public abstract class AToolbarButton
{
    protected readonly ToolbarButtonDef Def;
    protected readonly IThingTabRenderer Renderer;
    protected readonly Vector2 Size;

    public AToolbarButton(ToolbarButtonDef def, IThingTabRenderer renderer)
    {
        Def = def;
        Renderer = renderer;
        Text.Font = GameFont.Small;
        Size = Text.CalcSize(def.label);
    }

    public virtual void Render(ref Rect btnRect)
    {
        Text.Font = GameFont.Small;

        btnRect.width = Size.x + 16;

        if (IsLeftSide())
        {
            if (Widgets.ButtonText(btnRect, Def.label)) Action();
            btnRect.x += btnRect.width + 10;
        }
        else
        {
            btnRect.x -= btnRect.width + 10;
            if (Widgets.ButtonText(btnRect, Def.label)) Action();
        }

        if (!Def.description.NullOrEmpty()) TooltipHandler.TipRegion(btnRect, Def.description);
    }

    public abstract void Action();

    public bool IsLeftSide() => Def.Side == "left";

    public bool IsRightSide() => Def.Side == "right";
}