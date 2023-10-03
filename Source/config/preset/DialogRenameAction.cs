using System;
using Verse;

namespace BestApparel.config.preset;

public class DialogRenameAction : Dialog_Rename
{
    private readonly Action<string> _action;

    public DialogRenameAction(string name, Action<string> action)
    {
        curName = name;
        _action = action;
    }

    protected override void SetName(string name) => _action(name);
}