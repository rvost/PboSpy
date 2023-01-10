using BIS.Core.Config;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace PboExplorer.Modules.Core.Models;

class ConfigClassItem : ITreeItem
{
    public ConfigClassItem()
    {
        Name = "(root)";
    }

    public ConfigClassItem(ConfigClassItem parent, ParamClass entry, PboEntry file)
    {
        Parent = parent;
        Name = entry.Name;
        Apply(entry, file);
    }

    public ConfigClassItem Parent { get; }

    public string Name { get; }

    public string BaseClassName { get; private set; }

    private Dictionary<string, ConfigClassItem> ChildrenClasses { get; } = new Dictionary<string, ConfigClassItem>(StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    public List<Tuple<PboEntry, ParamClass>> Definitions { get; } = new List<Tuple<PboEntry, ParamClass>>();

    public ICollection<ITreeItem> Children
    {
        get
        {
            return GetAllChildren().OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }

    public IMetadata Metadata => new DictionaryPropertyGridAdapter<string, object>(Properties);

    public event PropertyChangedEventHandler PropertyChanged;

    private IEnumerable<ITreeItem> GetAllChildren()
    {
        return Merged(d => d.ChildrenClasses).Values.Cast<ITreeItem>();
    }

    public IEnumerable<KeyValuePair<string, object>> GetAllProperties()
    {
        return Merged(d => d.Properties).OrderBy(c => c.Key, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private Dictionary<string, T> Merged<T>(Func<ConfigClassItem, Dictionary<string, T>> get)
    {
        var thisData = get(this);
        var baseClass = GetBaseClass();
        if (baseClass != null)
        {
            var merged = new Dictionary<string, T>(baseClass.Merged(get), StringComparer.OrdinalIgnoreCase);
            foreach (var pair in thisData)
            {
                if (pair.Value == null)
                {
                    merged.Remove(pair.Key);
                }
                else
                {
                    merged[pair.Key] = pair.Value;
                }
            }
            return merged;
        }
        return thisData;
    }

    public ConfigClassItem GetBaseClass()
    {
        if (string.IsNullOrEmpty(BaseClassName))
        {
            return null;
        }
        var resolved = Parent.ResolveClassDirectThenDeep(BaseClassName);
        if (resolved == this)
        {
            return null; // FIXME !
        }
        return resolved;
    }

    public ConfigClassItem ResolveClassDirect(string className)
    {
        if (ChildrenClasses.TryGetValue(className, out var resolved))
        {
            return resolved;
        }
        var baseClass = GetBaseClass();
        if (baseClass != null)
        {
            return baseClass.ResolveClassDirect(className);
        }
        return null;
    }

    public ConfigClassItem ResolveClassDirectThenDeep(string className)
    {
        var resolved = ResolveClassDirect(className);
        if (resolved != null)
        {
            return resolved;
        }
        return Parent?.ResolveClassDirectThenDeep(className);
    }

    internal ICollection<ConfigClassItem> MergedView(IEnumerable<PboFile> files)
    {
        var paramFiles = new List<(ParamFile, PboEntry)>();
        foreach (var pbo in files)
        {
            IEnumerable<PboEntry> configFiles = pbo.AllEntries
                .Where(f => f.IsBinaryConfig() && !f.Name.EndsWith(".rvmat", StringComparison.OrdinalIgnoreCase));

            foreach (var file in configFiles)
            {
                try
                {
                    using var stream = file.GetStream();
                    paramFiles.Add((new ParamFile(stream), file));
                }
                catch (Exception e)
                {
                    Trace.TraceWarning("Unable to parse config: {0}", e); // TODO: Log or report error to user
                }
            }
        }

        foreach ((var paramClass, var entry) in paramFiles) // TODO: gracefully sort using CfgPatches
        {
            Apply(paramClass.Root, entry);
        }

        return ChildrenClasses.Values.OrderBy(c => c.Name).ToList();
    }

    private void Apply(ParamClass definition, PboEntry file)
    {
        BaseClassName = definition.BaseClassName;

        Definitions.Add(new Tuple<PboEntry, ParamClass>(file, definition));

        foreach (var entry in definition.Entries.OfType<ParamClass>())
        {
            if (ChildrenClasses.TryGetValue(entry.Name, out var existing))
            {
                existing.Apply(entry, file);
            }
            else
            {
                ChildrenClasses.Add(entry.Name, new ConfigClassItem(this, entry, file));
            }
        }
        foreach (var entry in definition.Entries.OfType<ParamValue>())
        {
            Properties[entry.Name] = entry.Value;
        }
        foreach (var entry in definition.Entries.OfType<ParamArray>())
        {
            Properties[entry.Name] = entry.Array;
        }
        foreach (var entry in definition.Entries.OfType<ParamDeleteClass>())
        {
            ChildrenClasses[entry.Name] = null;
        }
        // TODO: Consider ParamExternClass
    }
}