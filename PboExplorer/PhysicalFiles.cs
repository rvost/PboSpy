using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer
{
    class PhysicalFiles : ITreeItem
    {
        private readonly ObservableCollection<ITreeItem> files = new ObservableCollection<ITreeItem>();

        public PhysicalFiles()
        {
        }

        public string Name => "Other";

        public string FullPath { get; }

        public ICollection<ITreeItem> Children
        {
            get { return files; }
        }

        internal void AddEntry(PhysicalFile file)
        {
            files.Add(file);
        }
    }
}
