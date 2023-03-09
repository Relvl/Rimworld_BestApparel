using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui
{
    public static class UIUtils
    {
        public static void DrawLineAtTop(ref Rect inRect, bool usesScroll = true, int bottomMargin = 10)
        {
            DrawLineFull(BestApparel.COLOR_WHITE_A20, inRect.y, inRect.width - /*scrollbar width*/(usesScroll ? 16 : 0));
            if (bottomMargin > 0)
            {
                inRect.yMin += 10;
            }
        }

        public static void DrawLineFull(Color color, float y, float width)
        {
            GUI.color = color;
            Widgets.DrawLineHorizontal(0, y, width);
            GUI.color = Color.white;
        }

        public static void DrawButtonsRow(ref Rect inRect, int btnWidth, int btnHeight, int margin, params (string, Action)[] buttons)
        {
            var r = new Rect(0, inRect.y, 0, 0);
            foreach (var (label, action) in buttons)
            {
                DrawButtonRightOffset(ref r, label, action, btnWidth, btnHeight);
            }

            inRect.yMin += btnHeight + margin;
        }

        public static void DrawButtonRightOffset(ref Rect r, string label, Action onClick, int btnWidth = 85, int btnHeight = 24)
        {
            r.width = btnWidth;
            r.height = btnHeight;
            if (Widgets.ButtonText(r, label.Translate())) onClick();
            r.x += btnWidth + 10;
        }

        public static void RenderCheckboxLeft(ref Rect inRect, string label, bool state, Action<bool> onStateChanged, float maxWidth = 0)
        {
            var r = new Rect(inRect.x, inRect.y, maxWidth == 0 ? inRect.width : maxWidth, 24);
            if (Widgets.ButtonInvisible(r))
            {
                onStateChanged(!state);
            }

            Widgets.CheckboxDraw(inRect.x, inRect.y, state, false);
            r.x += r.height + 2;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(r, label);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        /** Returns total used height */
        public static float RenderCheckboxes(ref Rect inRect,
            string label,
            IReadOnlyList<Def> defs,
            ICollection<string> enabled,
            ICollection<string> disabled = null,
            int columnCount = 3)
        {
            if (enabled == null && disabled == null) return 0;
            var isMulti = enabled != null && disabled != null;
            var inRectStartsAt = inRect.yMin;

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            var labelTranslated = label.Translate();
            var labelWidth = Text.CalcSize(labelTranslated.RawText);
            var labelRect = new Rect(inRect.x, inRect.y, labelWidth.x, 20);
            Widgets.Label(labelRect, labelTranslated);
            TooltipHandler.TipRegion(labelRect, $"{label}.Tooltip".Translate());
            inRect.yMin += 36;

            var r = new Rect(inRect.x, inRect.y, 0, 16);

            Text.Anchor = TextAnchor.MiddleLeft;

            Text.Font = GameFont.Tiny;
            const int rowHeight = 20;
            var colWidth = inRect.width / columnCount - 2;
            for (var idx = 0; idx < defs.Count; idx++)
            {
                var def = defs[idx];
                var colIdx = idx % columnCount;
                var rowIdx = idx / columnCount;
                r.x = colWidth * colIdx + 2 * colIdx;
                r.width = Text.CalcSize(def.label).x + rowHeight + 6;
                r.y = inRect.y + rowHeight * rowIdx + 2 * rowIdx;
                r.height = rowHeight;

                var chkRect = new Rect(r.x, r.y, rowHeight, rowHeight);
                var isMouseOver = Mouse.IsOver(r);

                r.xMin += 4;

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

                    Widgets.CheckboxDraw(chkRect.x, chkRect.y, chkState, false, rowHeight);
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

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            inRect.yMin += defs.Count / columnCount * (rowHeight + 2);

            return inRect.yMin - inRectStartsAt;
        }
    }
}