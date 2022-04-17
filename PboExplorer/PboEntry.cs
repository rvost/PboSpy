using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BIS.Core.Config;
using BIS.PAA;
using BIS.PBO;

namespace PboExplorer
{
    class PboEntry : FileBase, ITreeItem
    {
        private readonly PBO pbo;

        public PboEntry(PBO pbo, FileEntry entry)
        {
            this.pbo = pbo;
            Name = Path.GetFileName(entry.FileName);
            Extension = Path.GetExtension(entry.FileName).ToLowerInvariant();
            Entry = entry;
        }

        public PBO PBO => pbo;

        public FileEntry Entry { get; }

        public override string Name { get; }

        public override string Extension { get; }

        public ICollection<ITreeItem> Children => null;

        public override string FullPath => pbo.Prefix + "\\" + Entry.FileName;

        public override int DataSize => Entry.DataSize;

        public override Stream GetStream()
        {
            return pbo.GetFileEntryStream(Entry);
        }

        internal void Extract(string fileName)
        {
            using (var stream = File.Create(fileName))
            {
                pbo.GetFileEntryStream(Entry).CopyTo(stream);
            }
        }
    }
}
