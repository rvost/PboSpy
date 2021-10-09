using System.Collections.Generic;
using System.IO;
using BIS.PBO;

namespace PboExplorer
{
    public class PboFile : ITreeItem
    {
        private readonly PBO pbo;
        private readonly PboDirectory root;

        public PboFile (PBO pbo)
        {
            this.pbo = pbo;
            this.root = GenerateRoot(pbo);
        }

        public PBO PBO => pbo;

        public string Name => pbo.FileName;

        public ICollection<ITreeItem> Children => root.Children;

        private static PboDirectory GenerateRoot(PBO pbo)
        {
            var root = new PboDirectory(null);
            foreach (var entry in pbo.FileEntries)
            {
                var parent = Path.GetDirectoryName(entry.FileName).Trim('/','\\');
                if (string.IsNullOrEmpty(parent))
                {
                    root.AddEntry(pbo, entry);
                }
                else
                {
                    GetDirectory(root, parent).AddEntry(pbo, entry);
                }
            }
            return root;
        }

        private static PboDirectory GetDirectory(PboDirectory root, string directory)
        {
            var parent = Path.GetDirectoryName(directory).Trim('/', '\\');
            if (string.IsNullOrEmpty(parent))
            {
                return root.GetOrAddDirectory(directory);
            }
            return GetDirectory(root, parent).GetOrAddDirectory(Path.GetFileName(directory));
        }

        internal void Extract(string fileName)
        {
            pbo.ExtractAllFiles(fileName);
        }
    }
}
