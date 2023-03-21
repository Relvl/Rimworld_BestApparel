using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BestApparel;

public class FeatureEnableDisable : IExposable
{
    private HashSet<string> _disabled = new();
    private HashSet<string> _enabled = new();

    public void Enable(string name)
    {
        _enabled.Add(name);
        _disabled.Remove(name);
    }

    public void Disable(string name)
    {
        _enabled.Remove(name);
        _disabled.Add(name);
    }

    public void Neutral(string name)
    {
        _enabled.Remove(name);
        _disabled.Remove(name);
    }

    public void EnableAll(IEnumerable<string> names)
    {
        _enabled.Clear();
        _enabled.AddRange(names);
        _disabled.Clear();
    }

    public void DisableAll(IEnumerable<string> names)
    {
        _enabled.Clear();
        _disabled.Clear();
        _disabled.AddRange(names);
    }

    public MultiCheckboxState GetState(string name)
    {
        if (_enabled.Contains(name)) return MultiCheckboxState.On;
        if (_disabled.Contains(name)) return MultiCheckboxState.Off;
        return MultiCheckboxState.Partial;
    }

    public MultiCheckboxState Toggle(string name, bool reverse = false)
    {
        switch (GetState(name))
        {
            case MultiCheckboxState.On:
                if (reverse)
                {
                    Neutral(name);
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                    return MultiCheckboxState.Partial;
                }

                Disable(name);
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                return MultiCheckboxState.Off;
            case MultiCheckboxState.Off:
                if (reverse)
                {
                    Enable(name);
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                    return MultiCheckboxState.On;
                }

                Neutral(name);
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                return MultiCheckboxState.Partial;
            default:
                if (reverse)
                {
                    Disable(name);
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                    return MultiCheckboxState.Off;
                }

                Enable(name);
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                return MultiCheckboxState.On;
        }
    }

    public void Clear()
    {
        _enabled.Clear();
        _disabled.Clear();
    }

    public bool IsAllowed(string name) => !_disabled.Contains(name) && _enabled.Contains(name);

    public bool IsCollectionAllowed<T>(ICollection<T> collection) where T : Def
    {
        // Every Enabled must be in the collection
        if (collection != null && _enabled.Count > 0 && !_enabled.All(e => collection.Any(c => c.defName == e))) return false;
        // No one Disabled must be in the collection
        if (collection != null && _disabled.Count > 0 && collection.Any(e => _disabled.Contains(e.defName))) return false;
        return true;
    }

    public void ExposeData()
    {
        Scribe_Collections.Look(ref _enabled, "Enabled");
        if (_enabled == null) _enabled = new HashSet<string>();
        Scribe_Collections.Look(ref _disabled, "Disabled");
        if (_disabled == null) _disabled = new HashSet<string>();
    }
}