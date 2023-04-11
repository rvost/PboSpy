using PboSpy.Interfaces;

namespace PboSpy.Modules.FileManager;

public class FileManagerEventArgs
{
    public IPersistentItem File { get; }

    public FileManagerEventArgs(IPersistentItem file)
    {
        File = file;
    }
}

public interface IFileManager
{
    ICollection<ITreeItem> FileTree { get; }

    event EventHandler<FileManagerEventArgs> FileLoaded;
    event EventHandler<FileManagerEventArgs> FileRemoved;

    Task LoadSupportedFiles(IEnumerable<string> fileNames);
    void Close(IPersistentItem file);
    void CloseAll();
}
