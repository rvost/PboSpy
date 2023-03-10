using System.IO;
using BIS.PBO;
using PboSpy.Interfaces;

namespace PboSpy.Models;
public class PboEntry : FileBase, ITreeItem
{
    private readonly PBO pbo;

    public PboEntry(PBO pbo, IPBOFileEntry entry)
    {
        this.pbo = pbo;
        Name = Path.GetFileName(entry.FileName);
        Extension = Path.GetExtension(entry.FileName).ToLowerInvariant();
        Entry = entry;
    }

    public PBO PBO => pbo;

    public IPBOFileEntry Entry { get; set; }

    public override string Name { get; }

    public override string Extension { get; }

    public ICollection<ITreeItem> Children => null;

    public override string FullPath => pbo.Prefix + "\\" + Entry.FileName;

    public override int DataSize => Entry.Size;

    public override Stream GetStream()
    {
        return Entry.OpenRead();
    }

    internal void Extract(string fileName)
    {
        using (var stream = File.Create(fileName))
        {
            using (var source = Entry.OpenRead())
            {
                source.CopyTo(stream);
            }
        }
    }

    public override bool Equals(object obj)
    {
        return obj is PboEntry entry &&
               FullPath == entry.FullPath;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FullPath);
    }

    public T Reduce<T>(ITreeItemTransformer<T> transformer)
        => transformer.Transform(this);
}
