using System.IO;
using PboExplorer.Interfaces;

namespace PboExplorer.Models;

public class PhysicalFile : FileBase, ITreeItem
{
    public PhysicalFile(string fullPath)
    {
        FullPath = fullPath;
    }

    public ICollection<ITreeItem> Children => null;

    public override string Extension => Path.GetExtension(FullPath);

    public override string Name => Path.GetFileName(FullPath);

    string ITreeItem.Name => FullPath;

    public override string FullPath { get; }

    public override int DataSize => (int)new FileInfo(FullPath).Length;

    public override Stream GetStream()
    {
        return File.OpenRead(FullPath);
    }

    public T Reduce<T>(ITreeItemTransformer<T> transformer) 
        => transformer.Transform(this);
}
