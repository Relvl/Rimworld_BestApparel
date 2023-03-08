using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui
{
    public static class UIUtils
    {
        public static void DrawLineFull(Color color, float y, float width)
        {
            GUI.color = color;
            Widgets.DrawLineHorizontal(0, y, width);
            GUI.color = Color.white;
        }

        public static void RenderCheckboxes(ref Rect inRect,
            string label,
            IReadOnlyList<Def> defs,
            ICollection<string> enabled,
            ICollection<string> disabled = null,
            int columnCount = 3)
        {
            if (enabled == null && disabled == null) return;
            var isMulti = enabled != null && disabled != null;

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            var labelRect = new Rect(inRect.x, inRect.y, inRect.width, 20);
            Widgets.Label(labelRect, label.Translate());
            TooltipHandler.TipRegion(labelRect, $"{label}.Tooltip".Translate());
            inRect.yMin += 26;

            var r = new Rect(inRect.x, inRect.y, 200, 16);

            Text.Anchor = TextAnchor.MiddleLeft;

            const int rowHeight = 24;
            var colWidth = inRect.width / columnCount - 2;
            for (var idx = 0; idx < defs.Count; idx++)
            {
                var def = defs[idx];
                var colIdx = idx % columnCount;
                var rowIdx = idx / columnCount;
                r.x = colWidth * colIdx + 2 * colIdx;
                r.width = colWidth;
                r.y = inRect.y + rowHeight * rowIdx + 2 * rowIdx;
                r.height = rowHeight;

                var chkRect = new Rect(r.x, r.y, rowHeight, rowHeight);
                var isMouseOver = Mouse.IsOver(r);

                r.xMax -= 24f;

                if (isMulti)
                {
                    var chkState = MultiCheckboxState.Partial;
                    if (disabled.Contains(def.defName))
                    {
                        chkState = MultiCheckboxState.Off;
                    }
                    else if (enabled.Contains(def.defName))
                    {
                        chkState = MultiCheckboxState.On;
                    }

                    if (Widgets.ButtonInvisible(r))
                    {
                        switch (chkState)
                        {
                            case MultiCheckboxState.On:
                                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                                enabled.Remove(def.defName);
                                disabled.Add(def.defName);
                                chkState = MultiCheckboxState.Off;
                                break;
                            case MultiCheckboxState.Off:
                                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                                enabled.Remove(def.defName);
                                disabled.Remove(def.defName);
                                chkState = MultiCheckboxState.Partial;
                                break;
                            default:
                                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                                enabled.Add(def.defName);
                                disabled.Remove(def.defName);
                                chkState = MultiCheckboxState.On;
                                break;
                        }
                    }

                    Widgets.CheckboxMulti(chkRect, chkState);
                }
                else
                {
                    var states = enabled ?? disabled;
                    var selected = states;
                    var chkState = selected.Contains(def.defName);

                    if (Widgets.ButtonInvisible(r))
                    {
                        if (chkState)
                        {
                            selected.Remove(def.defName);
                            chkState = false;
                        }
                        else
                        {
                            selected.Add(def.defName);
                            chkState = true;
                        }
                    }

                    Widgets.CheckboxDraw(chkRect.x, chkRect.y, chkState, false);
                }

                if (isMouseOver)
                {
                    GUI.DrawTexture(chkRect, TexUI.HighlightTex);
                    GUI.color = Color.yellow;
                    if (Prefs.DevMode)
                    {
                        TooltipHandler.TipRegion(r, $"defName: {def.defName}");
                    }
                }

                r.x += rowHeight + 2;
                Widgets.Label(r, def.label);

                GUI.color = Color.white;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            inRect.yMin += defs.Count / columnCount * (rowHeight + 2) + 20;
        }
    }
}