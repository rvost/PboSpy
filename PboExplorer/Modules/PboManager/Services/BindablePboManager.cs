using BIS.PBO;
using PboExplorer.Interfaces;
using PboExplorer.Models;
using System.Data;
using System.IO;

namespace PboExplorer.Modules.PboManager.Services;

[Export(typeof(IPboManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
class BindablePboManager : IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    
    public event EventHandler<PboManagerEventArgs> PboLoaded;
    public event EventHandler<PboManagerEventArgs> PboRemoved;

    public BindablePboManager()
    {
        FileTree = new BindableCollection<ITreeItem>();
    }

    public void LoadSupportedFiles(IEnumerable<string> paths)
    {
        // Split folders and files
        var lookup = paths
            .Select(path => Path.GetFullPath(path))
            .ToLookup(path => File.GetAttributes(path).HasFlag(FileAttributes.Directory));

        // Load folders
        lookup[true]
            .Select(path => LoadDirectory(path))
            .Apply(dir => FileTree.Add(dir));

        // Load files
        lookup[false]
            .Select(path => LoadFile(path))
            .Apply(file => FileTree.Add(file));

    }

    private ITreeItem LoadFile(string path)
    {
        if (IsPboFile(path))
        {
            return LoadPbo(path);
        }
        else
        {
            return LoadPhysicalFile(path);
        }
    }

    private PboFile LoadPbo(string path)
    {
        var pbo = new PboFile(new PBO(path, false));
        
        PboLoaded?.Invoke(this, new(pbo));
        
        return pbo;
    }

    private static PhysicalFile LoadPhysicalFile(string path)
        => new(Path.GetFullPath(path));

    private PhysicalDirectory LoadDirectory(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        var dir = new PhysicalDirectory(dirInfo.Name, dirInfo.FullName);

        dirInfo.EnumerateDirectories()
            .Select(inf => LoadDirectory(inf.FullName))
            .Apply(d => dir.Children.Add(d));

        GetSupportedFiles(dirInfo.FullName)
            .Select(file => LoadFile(file))
            .Apply(file => dir.Children.Add(file));

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
}