using PboSpy.Interfaces;

namespace PboSpy.Modules.FileManager;

public interface IFileLoader
{
    IFileLoader Next { get; set; }

    ITreeItem Load(string path);
}

public class FileLoader : IFileLoader
{
    public IFileLoader Next { get; set; }

    public virtual ITreeItem Load(string path) => Next?.Load(path);
}
