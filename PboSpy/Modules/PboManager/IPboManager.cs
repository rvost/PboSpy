using PboSpy.Interfaces;
using PboSpy.Models;

namespace PboSpy.Modules.PboManager;

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

    Task LoadSupportedFiles(IEnumerable<string> fileNames);
    void Close(ITreeSubnode file);
    void CloseAll();
}
