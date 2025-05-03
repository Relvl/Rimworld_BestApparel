using System;
using Verse;

namespace BestApparel.config.preset;

public class DialogRenameAction(string initialName, Action<string> onRenamed) : Dialog_Rename<DialogRenameAction.RenameActionProxy>(new RenameActionProxy(initialName))
{
    protected override void OnRenamed(string name) => onRenamed(name);

    public class RenameActionProxy(string name) : IRenameable
    {
        public string RenamableLabel { get; set; } = name;
        public string BaseLabel => RenamableLabel;
        public string InspectLabel => RenamableLabel;
    }
}