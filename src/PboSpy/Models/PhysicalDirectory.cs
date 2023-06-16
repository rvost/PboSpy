using PboSpy.Interfaces;

namespace PboSpy.Models;

public class PhysicalDirectory : ITreeItem, IPersistentItem
{
    private readonly BindableCollection<ITreeItem> files = new();

    public string Name { get; }
    public string Path => FullPath;

    public PhysicalDirectory(string name, string fullPath, ITreeItem parent = null)
    {
        Name = name;
        FullPath = fullPath;
        Parent = parent;
    }

    public string FullPath { get; }

    public ICollection<ITreeItem> Children
    {
        get { return files; }
    }

    public ITreeItem Parent { get; set; }
}
