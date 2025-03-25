using System;
using Verse;

namespace BestApparel.config.preset;

public class DialogRenameAction : Dialog_Rename<DialogRenameAction.RenameActionProxy>
{
    private readonly Action<string> onRenamed;

    public DialogRenameAction(string initialName, Action<string> onRenamed)
        : base(new RenameActionProxy(initialName))
    {
        this.onRenamed = onRenamed;
    }

    protected override void OnRenamed(string name)
    {
        onRenamed?.Invoke(name);
    }

    public class RenameActionProxy : IRenameable
    {
        private string name;

        public RenameActionProxy(string name)
        {
            this.name = name;
        }

        public string RenamableLabel
        {
            get => name;
            set => name = value;
        }

        public void TrySetName(string name)
        {
            this.name = name;
        }

        public string BaseLabel => name;

        public string InspectLabel => name;
    }
}