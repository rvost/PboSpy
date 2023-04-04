using BIS.PBO;
using PboSpy.Interfaces;
using System.IO;

namespace PboSpy.Models;

public class PboFile : ITreeItem, IPersistentItem
{
    private readonly PBO pbo;
    private readonly PboDirectory root;// TODO: Refactor

    public PboFile(PBO pbo, ITreeItem parent = null)
    {
        this.pbo = pbo;
        root = GenerateRoot(pbo, this);
        Parent = parent;
    }

    public PBO PBO => pbo;

    public string Name => pbo.FileName;

    public ICollection<ITreeItem> Children => root.Children;
    
    public ITreeItem Parent { get; }

    private static PboDirectory GenerateRoot(PBO pbo, ITreeItem parentPbo)
    {
        var root = new PboDirectory("", parentPbo);
        foreach (var entry in pbo.Files)
        {
            var parent = Path.GetDirectoryName(entry.FileName).Trim('/', '\\');
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

    internal IEnumerable<PboEntry> AllEntries => root.AllFiles;
}
