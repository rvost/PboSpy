using BIS.PBO;
using PboSpy.Interfaces;

namespace PboSpy.Modules.Pbo.Models;

public class PboDirectory : ITreeItem
{
    private readonly List<PboDirectory> directories = new();
    private readonly List<PboEntry> files = new();
    private List<ITreeItem> merged;

    public PboDirectory(string name, ITreeItem parent)
    {
        Name = name;
        Parent = parent;
    }

    public string Name { get; }
    public string Path => $"{Parent.Path}\\{Name}";
    public ITreeItem Parent { get; set; }

    public double DataSize => files.Sum(f => f.Entry.DiskSize) + directories.Sum(f => f.DataSize);

    public double UncompressedSize => files.Sum(f => f.Entry.Size) + directories.Sum(f => f.UncompressedSize);

    public ICollection<ITreeItem> Children
    {
        get
        {
            merged ??= directories.OrderBy(d => d.Name).Cast<ITreeItem>().Concat(files.OrderBy(f => f.Name)).ToList();
            return merged;
        }
    }

    internal void AddEntry(PBO pbo, IPBOFileEntry entry)
    {
        AddEntry(new PboEntry(pbo, entry, this));
    }

    internal void AddEntry(PboEntry entry)
    {
        files.Add(entry);
        merged = null;
    }

    internal PboDirectory GetOrAddDirectory(string childName)
    {
        var existing = directories.FirstOrDefault(d => string.Equals(d.Name, childName));
        if (existing == null)
        {
            existing = new PboDirectory(childName, this);
            directories.Add(existing);
            merged = null;
        }
        return existing;
    }

    internal IEnumerable<PboEntry> AllFiles => directories.SelectMany(d => d.AllFiles).Concat(files);
}
