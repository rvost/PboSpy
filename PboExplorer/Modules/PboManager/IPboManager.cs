using PboExplorer.Interfaces;

namespace PboExplorer.Modules.PboManager;

public interface IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    public ICollection<ITreeItem> ConfigTree { get; }

    void LoadSupportedFiles(IEnumerable<string> fileNames);
}
