using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace BestApparel.config.preset;

public class PresetManager
{
    public HashSet<PresetOption> Presets = [];

    public void MakeNewPreset(string tabId)
    {
        var rnd = new Random();
        int id;
        do
        {
            id = rnd.Next(100000, 1000000);
        } while (Presets.Any(p => p.ID == id));

        Find.WindowStack.Add(
            new DialogRenameAction(
                $"preset {id}",
                name =>
                {
                    Presets.Add(new PresetOption(id, name, tabId));
                    BestApparel.Config?.Mod?.WriteSettings();
                }
            )
        );
    }

    public void Remove(string tabId, int id)
    {
        Presets.RemoveWhere(p => p.ID == id && p.TabId == tabId);
        BestApparel.Config?.Mod?.WriteSettings();
    }

    public void ExposeData()
    {
        if (Scribe.EnterNode("Presets"))
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var preset in Presets)
                            if (Scribe.EnterNode("Preset"))
                                try
                                {
                                    preset.ExposeData();
                                }
                                finally
                                {
                                    Scribe.ExitNode();
                                }

                        break;
                    case LoadSaveMode.LoadingVars:
                        Presets.Clear();

                        var enumerator2 = Scribe.loader.curXmlParent.ChildNodes.GetEnumerator();
                        try
                        {
                            while (enumerator2!.MoveNext())
                            {
                                var preset = ScribeExtractor.SaveableFromNode<PresetOption>((XmlNode)enumerator2.Current, []);
                                Presets.Add(preset);
                            }
                        }
                        finally
                        {
                            if (enumerator2 is IDisposable disposable)
                                disposable.Dispose();
                        }

                        /*foreach (XmlElement element in Scribe.loader.curXmlParent)
                        {
                            if (Scribe.EnterNode(element.Name))
                            {
                                try
                                {
                                    var preset = new PresetOption();
                                    preset.ExposeData();
                                    Presets.Add(preset);
                                }
                                finally
                                {
                                    Scribe.ExitNode();
                                }
                            }
                        }*/
                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }

        Presets ??= [];
    }
}