using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel.config.preset;

public class PresetOption : IExposable
{
    public const int Size = 16;

    public int ID = 0;
    public string TabId = "";

    private string _name = "";
    private FloatMenuOption _option;
    private TabConfig _tabConfig;

    public FloatMenuOption Option =>
        _option ??= new FloatMenuOption( //
            _name,
            OnSelect,
            null,
            Color.white,
            MenuOptionPriority.Default,
            null,
            null,
            Size * 3,
            OnGUI,
            null,
            true,
            0,
            HorizontalJustification.Left,
            true
        );

    public PresetOption()
    {
        _tabConfig = new TabConfig();
    }

    public PresetOption(int id, string name, string tabId)
    {
        ID = id;
        _name = name;
        TabId = tabId;
        Store();
    }

    private void Store()
    {
        _tabConfig = new TabConfig(BestApparel.GetTabConfig(TabId));
    }

    private void ConfirmOverride()
    {
        Store();
        BestApparel.Config?.Mod?.WriteSettings();
    }

    private void ConfirmRemove()
    {
        BestApparel.Config.PresetManager.Remove(TabId, ID);
    }

    private void OnSelect() => BestApparel.ApplyTabConfig(TabId, _tabConfig);

    private bool OnGUI(Rect rect)
    {
        var updateRect = new Rect(rect.x, rect.y + rect.height / 2f - Size / 2f, Size, Size);
        TooltipHandler.TipRegion(updateRect, "Save current");
        if (Widgets.ButtonImage(updateRect, TexButton.Copy, Color.white, Color.yellow))
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation($"Override preset '{_name}'?", ConfirmOverride, true, "Override".Translate()));
            return true;
        }

        updateRect.x += Size + 8;
        TooltipHandler.TipRegion(updateRect, "Remove");
        if (Widgets.ButtonImage(updateRect, TexButton.CloseXSmall, Color.red, Color.yellow))
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation($"Delete preset '{_name}'?", ConfirmRemove, true, "Delete".Translate()));
            return true;
        }

        return false;
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref ID, "Id");
        Scribe_Values.Look(ref _name, "Name");
        Scribe_Values.Look(ref TabId, "TabId");
        _tabConfig.ExposeData();
    }
}