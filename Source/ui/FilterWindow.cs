using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel.ui
{
    public class FilterWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(650, 500);

        private readonly MainTabWindow _parent;

        public FilterWindow(MainTabWindow parent)
        {
            // base
            doCloseX = true;
            draggable = true;
            // this
            _parent = parent;
        }

        public override void DoWindowContents(Rect inRect)
        {
            RenderCheckboxes(ref inRect, "BestApparel.Label.LayerList", _parent.ApparelLayers, _parent.Config.EnabledLayers, _parent.Config.DisabledLayers);
            RenderCheckboxes(ref inRect, "BestApparel.Label.BodyPartList", _parent.ApparelBodyParts, _parent.Config.EnabledBodyParts, _parent.Config.DisabledBodyParts);
            // todo! kid/adult

            const int btnHeight = 24;
            const int btnWidth = 120;
            var btnRect = new Rect(0, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
            if (Widgets.ButtonText(btnRect, "BestApparel.Btn.Resort".Translate()))
            {
                _parent.Resort();
            }

            btnRect.x += btnWidth + 10;
            btnRect.width = inRect.width - btnRect.x;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = ModEntrance.COLOR_WHITE_A20;
            Widgets.Label(btnRect, "BestApparel.Btn.Resort.Additional".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            btnRect = new Rect(windowRect.width - Margin * 2 - btnWidth, windowRect.height - Margin * 2 - btnHeight, btnWidth, btnHeight);
            if (Widgets.ButtonText(btnRect, "BestApparel.Btn.Defaults".Translate()))
            {
                _parent.Config.Defaults();
                _parent.Resort();
            }
        }

        private static void RenderCheckboxes(ref Rect inRect, string label, IReadOnlyList<Def> defs, ICollection<string> enabled, ICollection<string> disabled)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            var labelRect = new Rect(inRect.x, inRect.y, inRect.width, 20);
            Widgets.Label(labelRect, label.Translate());
            TooltipHandler.TipRegion(labelRect, $"{label}.Tooltip".Translate());
            inRect.yMin += 26;

            var r = new Rect(inRect.x, inRect.y, 200, 16);

            Text.Anchor = TextAnchor.MiddleLeft;

            const int cols = 3;
            const int rowHeight = 24;
            var colWidth = inRect.width / 3 - 4;
            for (var idx = 0; idx < defs.Count; idx++)
            {
                var bodyPartDef = defs[idx];
                var colIdx = idx % cols;
                var rowIdx = idx / cols;
                r.x = colWidth * colIdx + 2 * colIdx;
                r.width = colWidth;
                r.y = inRect.y + rowHeight * rowIdx + 2 * rowIdx;
                r.height = rowHeight;

                var chkRect = new Rect(r.x, r.y, rowHeight, rowHeight);
                var isMouseOver = Mouse.IsOver(r);

                r.xMax -= 24f;

                var chkState = MultiCheckboxState.Partial;
                if (disabled.Contains(bodyPartDef.defName))
                {
                    chkState = MultiCheckboxState.Off;
                }
                else if (enabled.Contains(bodyPartDef.defName))
                {
                    chkState = MultiCheckboxState.On;
                }

                if (Widgets.ButtonInvisible(r))
                {
                    switch (chkState)
                    {
                        case MultiCheckboxState.On:
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            enabled.Remove(bodyPartDef.defName);
                            disabled.Add(bodyPartDef.defName);
                            chkState = MultiCheckboxState.Off;
                            break;
                        case MultiCheckboxState.Off:
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            enabled.Remove(bodyPartDef.defName);
                            disabled.Remove(bodyPartDef.defName);
                            chkState = MultiCheckboxState.Partial;
                            break;
                        default:
                            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                            enabled.Add(bodyPartDef.defName);
                            disabled.Remove(bodyPartDef.defName);
                            chkState = MultiCheckboxState.On;
                            break;
                    }
                }

                Widgets.CheckboxMulti(chkRect, chkState);

                if (isMouseOver)
                {
                    GUI.DrawTexture(chkRect, TexUI.HighlightTex);
                    GUI.color = Color.yellow;
                }

                r.x += rowHeight + 2;
                Widgets.Label(r, bodyPartDef.label);

                GUI.color = Color.white;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            inRect.yMin += defs.Count / cols * (rowHeight + 2) + 20;
        }

        public override void PreClose()
        {
            _parent.Resort();
        }
    }
}