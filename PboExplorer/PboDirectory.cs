using System;
using System.Collections.Generic;
using System.Linq;
using BIS.PBO;

namespace PboExplorer
{
    internal class PboDirectory : ITreeItem
    {
        private readonly List<PboDirectory> directories = new List<PboDirectory>();
        private readonly List<PboEntry> files = new List<PboEntry>();
        private List<ITreeItem> merged;

        public PboDirectory(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int DataSize => files.Sum(f => f.Entry.DataSize) + directories.Sum(f => f.DataSize);

        public int UncompressedSize => files.Sum(f => f.Entry.IsCompressed ? f.Entry.UncompressedSize : f.Entry.DataSize) + directories.Sum(f => f.UncompressedSize);

        public ICollection<ITreeItem> Children
        {
            get
            {
                if (merged == null)
                {
                    merged = directories.OrderBy(d => d.Name).Cast<ITreeItem>().Concat(files.OrderBy(f => f.Name)).ToList();
                }
                return merged;
            }
        }

        internal void AddEntry(PBO pbo, FileEntry entry)
        {
            files.Add(new PboEntry(pbo, entry));
            merged = null;
        }

        internal PboDirectory GetOrAddDirectory(string childName)
        {
            var existing = directories.FirstOrDefault(d => string.Equals(d.Name, childName));
            if (existing == null)
            {
                existing = new PboDirectory(childName);
                directories.Add(existing);
                merged = null;
            }
            return existing;
        }
    }
}