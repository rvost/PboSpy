using PboExplorer.Interfaces;

namespace PboExplorer.Models;

public class PhysicalDirectory : ITreeItem
{
    private readonly ObservableCollection<ITreeItem> files = new ObservableCollection<ITreeItem>();

    public string Name { get; }

    public PhysicalDirectory(string name, string fullPath)
    {
        Name = name;
        FullPath = fullPath;
    }

    public string FullPath { get; }

    public ICollection<ITreeItem> Children
    {
        get { return files; }
    }
    
    public T Reduce<T>(ITreeItemTransformer<T> transformer)
        => transformer.Transform(this);
}
