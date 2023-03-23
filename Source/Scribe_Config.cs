using System.Collections.Generic;
using System.Xml;
using Verse;

namespace BestApparel;

public static class Scribe_Config
{
    public static void LookDictionary<TV>(ref Dictionary<string, TV> dict, string label)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                dict ??= new Dictionary<string, TV>();

                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, value) in dict)
                        {
                            Scribe.saver.WriteElement(key, value.ToString());
                        }

                        break;
                    case LoadSaveMode.LoadingVars:
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                        {
                            dict[child.Name] = ScribeExtractor.ValueFromNode<TV>(child, default);
                        }

                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            dict = new Dictionary<string, TV>();
        }
    }

    public static void LookStringList(ref List<string> list, string label)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                list ??= new List<string>();

                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var element in list) Scribe.saver.WriteElement(element, "");
                        break;
                    case LoadSaveMode.LoadingVars:
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children) list.Add(child.Name);
                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            list ??= new List<string>();
        }
    }

    public static void LookListDictionary(ref Dictionary<string, List<string>> dict, string label)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                dict ??= new Dictionary<string, List<string>>();

                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, list) in dict)
                        {
                            var innerlist = list ?? new List<string>();
                            LookStringList(ref innerlist, key);
                        }

                        break;
                    case LoadSaveMode.LoadingVars:
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                        {
                            var innerList = new List<string>();
                            LookStringList(ref innerList, child.Name);
                            dict[child.Name] = innerList;
                        }

                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            dict = new Dictionary<string, List<string>>();
        }
    }

    public static void LookDeepDictionary<TV>(ref Dictionary<string, Dictionary<string, TV>> dict, string label)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                dict ??= new Dictionary<string, Dictionary<string, TV>>();

                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, value) in dict)
                        {
                            var inner = value;
                            LookDictionary(ref inner, key);
                        }

                        break;
                    case LoadSaveMode.LoadingVars:
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                        {
                            var inner = new Dictionary<string, TV>();
                            LookDictionary(ref inner, child.Name);
                            dict[child.Name] = inner;
                        }

                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            dict = new Dictionary<string, Dictionary<string, TV>>();
        }
    }
}