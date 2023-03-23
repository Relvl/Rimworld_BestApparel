using System.Collections.Generic;
using System.Xml;
using Verse;

namespace BestApparel;

public static class Scribe_Config
{
    public static void LookDictionary<TV>(ref Dictionary<string, TV> dict, string label)
    {
        dict ??= new Dictionary<string, TV>();
        if (Scribe.EnterNode(label))
        {
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, value) in dict)
                            Scribe.saver.WriteElement(key, value.ToString());
                        break;
                    case LoadSaveMode.LoadingVars:
                        dict.Clear();
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                            dict[child.Name] = ScribeExtractor.ValueFromNode<TV>(child, default);
                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
    }

    public static void LookListString(ref List<string> list, string label)
    {
        list ??= new List<string>();
        if (Scribe.EnterNode(label))
        {
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var element in list) Scribe.saver.WriteElement(element, "");
                        break;
                    case LoadSaveMode.LoadingVars:
                        list.Clear();
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
    }

    public static void LookDictionaryList(ref Dictionary<string, List<string>> dict, string label)
    {
        dict ??= new Dictionary<string, List<string>>();
        if (Scribe.EnterNode(label))
        {
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, list) in dict)
                        {
                            var innerlist = list ?? new List<string>();
                            LookListString(ref innerlist, key);
                        }

                        break;
                    case LoadSaveMode.LoadingVars:
                        dict.Clear();
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                        {
                            var innerList = new List<string>();
                            LookListString(ref innerList, child.Name);
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
    }

    public static void LookDictionaryDeep2<TV>(ref Dictionary<string, Dictionary<string, TV>> dict, string label)
    {
        dict ??= new Dictionary<string, Dictionary<string, TV>>();
        if (Scribe.EnterNode(label))
        {
            try
            {
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
                        dict.Clear();
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
    }

    // todo! чёт оно слишком дико выглядит
    public static void LookDictionaryDeep3<TV>(ref Dictionary<string, Dictionary<string, Dictionary<string, TV>>> dict, string label)
    {
        dict ??= new Dictionary<string, Dictionary<string, Dictionary<string, TV>>>();
        if (Scribe.EnterNode(label))
        {
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (key, value) in dict)
                        {
                            var inner = value;
                            LookDictionaryDeep2(ref inner, key);
                        }

                        break;
                    case LoadSaveMode.LoadingVars:
                        dict.Clear();
                        var children = Scribe.loader.curXmlParent;
                        foreach (XmlElement child in children)
                        {
                            var inner = new Dictionary<string, Dictionary<string, TV>>();
                            LookDictionaryDeep2(ref inner, child.Name);
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
    }
}