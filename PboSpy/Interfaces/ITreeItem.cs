namespace PboSpy.Interfaces;

public interface ITreeItem
{
    string Name { get; }

    string Path { get; }
    
    ITreeItem Parent { get; set; }
    
    ICollection<ITreeItem> Children { get; }
}
