using BIS.Core.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PboExplorer
{
    internal class ConfigClassItem : ITreeItem
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

        public Dictionary<string, ConfigClassItem> ChildrenClasses { get; } = new Dictionary<string, ConfigClassItem>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public List<Tuple<PboEntry, ParamClass>> Definitions { get; } = new List<Tuple<PboEntry, ParamClass>>();

        public ICollection<ITreeItem> Children => ChildrenClasses.Values.OrderBy(c => c.Name).Cast<ITreeItem>().ToList(); // TODO : Base class

        public IEnumerable<KeyValuePair<string, object>> GetAllProperties()
        {
            return Properties.OrderBy(c => c.Key).ToList(); // TODO : Base class
        }

        internal static ICollection<ConfigClassItem> MergedView(IEnumerable<PboFile> files)
        {
            var paramFiles = new List<Tuple<ParamFile, PboEntry>>();
            foreach (var pbo in files)
            {
                foreach(var file in pbo.AllEntries.OfType<PboEntry>().Where(f => f.IsBinaryConfig() && !f.Name.EndsWith(".rvmat", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        using (var stream = file.GetStream())
                        {
                            paramFiles.Add(new Tuple<ParamFile, PboEntry>(new ParamFile(stream), file));

                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning("Unable to parse config: {0}", e);
                    }
                }
            }

            var root = new ConfigClassItem();
            foreach(var file in paramFiles) // gracefully sort using CfgPatches
            {
                root.Apply(file.Item1.Root, file.Item2);
            }
            return root.ChildrenClasses.Values.OrderBy(c => c.Name).ToList();
        }

        private void Apply(ParamClass definition, PboEntry file)
        {
            Definitions.Add(new Tuple<PboEntry, ParamClass>(file, definition));

            foreach(var entry in definition.Entries.OfType<ParamClass>())
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

        }
    }
}