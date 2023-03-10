using System.IO;
using BIS.PBO;
using PboSpy.Interfaces;

namespace PboSpy.Models;

public class PboFile : ITreeSubnode
{
    private readonly PBO pbo;
    private readonly PboDirectory root;

    public PboFile(PBO pbo, ITreeItem parent = null)
    {
        this.pbo = pbo;
        root = GenerateRoot(pbo);
        Parent = parent;
    }

    public PBO PBO => pbo;

    public string Name => pbo.FileName;

    public ICollection<ITreeItem> Children => root.Children;
    public ITreeItem Parent { get; }

    private static PboDirectory GenerateRoot(PBO pbo)
    {
        var root = new PboDirectory(null);
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

    internal static PboDirectory MergedView(IEnumerable<PboFile> files)
    {
        var masterRoot = new PboDirectory(null);
        foreach (var entry in files.SelectMany(f => f.AllEntries))
        {
            GetDirectory(masterRoot, Path.GetDirectoryName(entry.FullPath).Trim('/', '\\')).AddEntry(entry);
        }
        return masterRoot;
    }

    internal void Extract(string fileName)
    {
        pbo.ExtractAllFiles(fileName);
    }

    internal IEnumerable<PboEntry> AllEntries => root.AllFiles;

    public T Reduce<T>(ITreeItemTransformer<T> transformer)
        => transformer.Transform(this);
}
