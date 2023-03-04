using PboExplorer.Interfaces;

namespace PboExplorer.Models;

public class PhysicalDirectory : ITreeSubnode
{
    private readonly BindableCollection<ITreeItem> files = new();

    public string Name { get; }

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

    public ITreeItem Parent { get; }

    public T Reduce<T>(ITreeItemTransformer<T> transformer)
        => transformer.Transform(this);
}
