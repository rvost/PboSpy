using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer
{
    class PhysicalFile : FileBase, ITreeItem
    {
        public PhysicalFile (string fullPath)
        {
            FullPath = fullPath;
        }

        public ICollection<ITreeItem> Children => null;

        public override string Extension => Path.GetExtension(FullPath);

        public override string Name => Path.GetFileName(FullPath);

        string ITreeItem.Name => FullPath;

        public override string FullPath { get; }

        public override Stream GetStream()
        {
            return File.OpenRead(FullPath);
        }
    }
}
