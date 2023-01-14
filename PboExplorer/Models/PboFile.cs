using System.ComponentModel;
using System.IO;
using BIS.PBO;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace PboExplorer.Models;

class PboFile : ITreeItem
{
    private readonly PBO pbo;
    private readonly PboDirectory root;
    private readonly Lazy<PboMetadata> metadata;

    public PboFile(PBO pbo)
    {
        this.pbo = pbo;
        root = GenerateRoot(pbo);
        metadata = new Lazy<PboMetadata>(() => new(this));
    }

    public IMetadata Metadata
    {
        get => metadata.Value;
    }

    public PBO PBO => pbo;

    public string Name => pbo.FileName;

    public ICollection<ITreeItem> Children => root.Children;

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

}

class PboMetadata : IMetadata
{
    [Description("Number of files in PBO")]
    public string Path { get; set; }

    [Description("Size of PBO on disk")]
    public string Size { get; set; }

    [Description("Number of files in PBO")]
    public int Entries { get; set; }

    [Description("PBO Prefix")]
    public string Prefix { get; set; }

    [Editor(typeof(CollectionEditor), typeof(CollectionEditor))]
    public ICollection<KeyValuePair<string, string>> Properties { get; }

    public PboMetadata(PboFile file)
    {
        Path = file.PBO.PBOFilePath;
        Size = Formatters.FormatSize(new FileInfo(file.PBO.PBOFilePath).Length);
        Entries = file.PBO.Files.Count;
        Prefix = file.PBO.Prefix;
        Properties = file.PBO.PropertiesPairs;
    }
}