using PboExplorer.Interfaces;
using PboExplorer.Models;

namespace PboExplorer.Modules.PboManager;

// TODO: Generalize for all supported files
public class PboManagerEventArgs
{
    public PboFile File { get; }

    public PboManagerEventArgs(PboFile file)
    {
        File = file;
    }
}

public interface IPboManager
{
    ICollection<ITreeItem> FileTree { get; }

    event EventHandler<PboManagerEventArgs> PboLoaded;
    event EventHandler<PboManagerEventArgs> PboRemoved;

    void LoadSupportedFiles(IEnumerable<string> fileNames);
}
