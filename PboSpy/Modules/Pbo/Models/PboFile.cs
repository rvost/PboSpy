using BIS.PBO;
using PboSpy.Interfaces;
using System.Text.RegularExpressions;

namespace PboSpy.Modules.Pbo.Models;

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

    public string Path => System.IO.Path.GetFullPath(pbo.PBOFilePath);

    public ICollection<ITreeItem> Children => root.Children;

    public ITreeItem Parent { get; set; }

    private static PboDirectory GenerateRoot(PBO pbo, ITreeItem parentPbo)
    {
        var root = new PboDirectory("", parentPbo);
        foreach (var entry in pbo.Files.Where(f => f.Size > 4))
        {
            var sanitizedName = SanitizeFileName(entry.FileName);
            var parent = System.IO.Path.GetDirectoryName(sanitizedName)?.Trim('/', '\\');
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
        var parent = System.IO.Path.GetDirectoryName(directory)?.Trim('/', '\\');
        if (string.IsNullOrEmpty(parent))
        {
            return root.GetOrAddDirectory(directory);
        }
        return GetDirectory(root, parent).GetOrAddDirectory(System.IO.Path.GetFileName(directory));
    }

    internal void Extract(string fileName)
    {
        pbo.ExtractAllFiles(fileName);
    }

    internal IEnumerable<PboEntry> AllEntries => root.AllFiles;

    private static string SanitizeFileName(string name)
    {
        if (name != null)
        {
            string pattern = @"\.+\\|\\{2,}";
            var res = Regex.Replace(name, pattern, "\\");
            return res;
        }
        return null;
    }
}
