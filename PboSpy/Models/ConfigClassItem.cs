using BIS.Core.Config;
using BIS.PBO;
using PboSpy.Interfaces;
using System.ComponentModel;
using System.Diagnostics;

namespace PboSpy.Models;

public class ConfigClassItem : ITreeItem
{
    public const string ROOT = "(root)";
    
    public ConfigClassItem()
    {
        Name = ROOT;
    }

    public ConfigClassItem(ConfigClassItem parent, ParamClass entry, PboEntry file)
    {
        Parent = parent;
        Name = entry.Name;
        PBO = file.PBO;
        Apply(entry, file);
    }

    public ConfigClassItem Parent { get; }

    // TODO: Refactor
    public PBO PBO { get; }

    public string Name { get; }

    public string BaseClassName { get; private set; }

    private Dictionary<string, ConfigClassItem> ChildrenClasses { get; } = new Dictionary<string, ConfigClassItem>(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    public Dictionary<PboEntry, ParamClass> Definitions { get; } = new();

    public ICollection<ITreeItem> Children
    {
        get
        {
            return GetAllChildren().OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }

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

    public ICollection<ConfigClassItem> MergePbo(PboFile pbo)
    {
        var paramFiles = new List<(ParamFile, PboEntry)>();

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

        foreach ((var paramClass, var entry) in paramFiles) // TODO: gracefully sort using CfgPatches
        {
            Apply(paramClass.Root, entry);
        }

        return ChildrenClasses.Values.OrderBy(c => c.Name).ToList();
    }

    public ICollection<ConfigClassItem> RemovePbo(PboFile pbo)
    {
        var paramFiles = new List<(ParamFile, PboEntry)>();

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

        foreach ((var paramClass, var entry) in paramFiles)
        {
            Remove(paramClass.Root, entry);
        }

        return ChildrenClasses.Values.OrderBy(c => c.Name).ToList();
    }

    private void Apply(ParamClass definition, PboEntry file)
    {
        BaseClassName = definition.BaseClassName;

        if (Definitions.TryAdd(file, definition))
        {
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

    private void Remove(ParamClass definition, PboEntry file)
    {
        if (Definitions.Remove(file))
        {
            foreach (var entry in definition.Entries.OfType<ParamClass>())
            {
                if (ChildrenClasses.TryGetValue(entry.Name, out var existing))
                {
                    existing.Remove(entry, file);

                    if (!existing.ChildrenClasses.Any())
                    {
                        ChildrenClasses.Remove(entry.Name);
                    }
                }
            }
            foreach (var entry in definition.Entries.OfType<ParamValue>())
            {
                Properties.Remove(entry.Name);
            }
            foreach (var entry in definition.Entries.OfType<ParamArray>())
            {
                Properties.Remove(entry.Name);
            }

            // TODO: Account for ParamDeleteClass
        }
    }

    public T Reduce<T>(ITreeItemTransformer<T> transformer)
        => transformer.Transform(this);
}