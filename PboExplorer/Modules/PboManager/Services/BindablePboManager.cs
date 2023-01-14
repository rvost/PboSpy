using BIS.PBO;
using PboExplorer.Interfaces;
using PboExplorer.Models;
using System.Data;
using System.IO;

namespace PboExplorer.Modules.PboManager.Services;

// TODO: Consider moving tree building to separate utility class
[Export(typeof(IPboManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
class BindablePboManager : IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    public ICollection<ITreeItem> ConfigTree { get; }
    public ConfigClassItem ConfigRoot { get; }

    public BindablePboManager()
    {
        FileTree = new BindableCollection<ITreeItem>();
        ConfigTree = new BindableCollection<ITreeItem>();
        ConfigRoot = new();
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
        // TODO: Implement GenerateMergedConfig override which takes one file
        GenerateMergedConfig(new[] { pbo });
        return pbo;
    }

    private static PhysicalFile LoadPhysicalFile(string path)
        => new PhysicalFile(Path.GetFullPath(path));

    private PhysicalDirectory LoadDirectory(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        var dir = new PhysicalDirectory(dirInfo.Name);

        dirInfo.EnumerateDirectories()
            .Select(inf => LoadDirectory(inf.FullName))
            .Apply(d => dir.Children.Add(d));

        GetSupportedFiles(dirInfo.FullName)
            .Select(file => LoadFile(file))
            .Apply(file => dir.Children.Add(file));

        return dir;
    }

    private void GenerateMergedConfig(IEnumerable<PboFile> files)
    {
        var configClasses = ConfigRoot.MergedView(files);

        // TODO: Refactor ConfigTree update
        ConfigTree.Clear();
        foreach (var configClass in configClasses)
        {
            ConfigTree.Add(configClass);
        }
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