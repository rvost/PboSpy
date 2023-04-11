using BIS.PBO;
using PboSpy.Interfaces;
using PboSpy.Models;
using System.Data;
using System.IO;

namespace PboSpy.Modules.PboManager.Services;

[Export(typeof(IPboManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
class BindablePboManager : IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }

    public event EventHandler<FileManagerEventArgs> FileLoaded;
    public event EventHandler<FileManagerEventArgs> FileRemoved;

    public BindablePboManager()
    {
        FileTree = new BindableCollection<ITreeItem>();
    }

    public async Task LoadSupportedFiles(IEnumerable<string> paths)
    {
        var res = await Task.Run(() => paths.Select(x => IsDirectory(x) ? LoadDirectory(x, null) : LoadFile(x, null)))
            .ConfigureAwait(false);

        res.Apply(item => FileTree.Add(item));
    }

    // TODO: Remove recursion
    public void Close(IPersistentItem file)
    {
        var childrenToClose = file.Children?.OfType<IPersistentItem>().ToList() ?? new();
        childrenToClose.Apply(child => Close(child));

        if (file.Parent is null)
        {
            FileTree.Remove(file);
        }
        else
        {
            file.Parent.Children.Remove(file);
        }

        if (file is IPersistentItem persistentFile)
        {
            OnFileRemoved(persistentFile);
        }
    }

    // TODO: Remove recursion
    public void CloseAll()
    {
        var toClose = FileTree.OfType<IPersistentItem>().ToList() ?? new();
        toClose.Apply(item => Close(item));
    }

    private ITreeItem LoadFile(string path, ITreeItem parent)
    {
        IPersistentItem file = IsPboFile(path) ? new PboFile(new PBO(path, false), parent)
            : new PhysicalFile(Path.GetFullPath(path), parent);
        OnFileLoaded(file);
        return file;
    }

    private PhysicalDirectory LoadDirectory(string path, ITreeItem parent)
    {
        var dirInfo = new DirectoryInfo(path);

        var dir = new PhysicalDirectory(dirInfo.Name, dirInfo.FullName, parent);

        dirInfo.EnumerateDirectories()
            .Select(inf => LoadDirectory(inf.FullName, dir))
            .Apply(d => dir.Children.Add(d));

        GetSupportedFiles(dirInfo.FullName)
            .Select(file => LoadFile(file, dir))
            .Apply(file => dir.Children.Add(file));

        OnFileLoaded(dir);

        return dir;
    }

    private static bool IsPboFile(string path)
        => string.Equals(Path.GetExtension(path), ".pbo", StringComparison.OrdinalIgnoreCase);

    private static IEnumerable<string> GetSupportedFiles(string arg)
    {
        HashSet<string> _supportedExtensions = new() { ".pbo", ".paa", ".rvmat", ".bin",
        ".pac", ".p3d", ".wrp", ".sqm", ".bisign", ".bikey", ".cpp", ".hpp" };

        return Directory.EnumerateFiles(arg, "*.*", SearchOption.TopDirectoryOnly)
            .Where(path => _supportedExtensions.Contains(Path.GetExtension(path)));
    }

    private static bool IsDirectory(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    private void OnFileLoaded(IPersistentItem file) 
        => FileLoaded?.Invoke(this, new(file));

    private void OnFileRemoved(IPersistentItem file)
        => FileRemoved?.Invoke(this, new(file));
}
