using PboSpy.Interfaces;
using System.Data;
using System.IO;

namespace PboSpy.Modules.FileManager.Services;

[Export(typeof(IFileManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
class FileManager : IFileManager
{
    private IFileLoader _loaderChain;
    public ICollection<ITreeItem> FileTree { get; }

    public event EventHandler<FileManagerEventArgs> FileLoaded;
    public event EventHandler<FileManagerEventArgs> FileRemoved;

    [ImportingConstructor]
    public FileManager([ImportMany(typeof(IFileLoader))] IEnumerable<Lazy<IFileLoader>> loaderEports)
    {
        _loaderChain = BuildLoaderChain(loaderEports);
        FileTree = new BindableCollection<ITreeItem>();
    }

    public async Task LoadSupportedFiles(IEnumerable<string> paths)
    {
        var res = await Task.Run(() =>
        {
            var flat = ExpandPaths(paths)
            .Select(Path.GetFullPath)
            .Select(path => _loaderChain.Load(path))
            .Where(x => x != null)
            .ToList();

            foreach (var item in flat.OfType<IPersistentItem>())
            {
                OnFileLoaded(item);
            }

            return ToHierarchy(flat);
        }).ConfigureAwait(false);

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

    private static bool IsDirectory(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    private void OnFileLoaded(IPersistentItem file)
        => FileLoaded?.Invoke(this, new(file));

    private void OnFileRemoved(IPersistentItem file)
        => FileRemoved?.Invoke(this, new(file));

    private static IFileLoader BuildLoaderChain(IEnumerable<Lazy<IFileLoader>> exports)
    {
        var supportedExtensions = new string[] { ".pbo", ".paa", ".rvmat", ".bin",
        ".pac", ".p3d", ".wrp", ".sqm", ".bisign", ".bikey", ".cpp", ".hpp" }; // TODO: Make configurable

        var loaders = exports.Select(o => o.Value);
        var stack = new Stack<IFileLoader>(loaders);
        stack.Push(new PhysicalFileLoader(supportedExtensions));
        stack.Push(new PhysicalDirectoryLoader());

        var chain = stack.Pop();

        foreach (var loader in stack)
        {
            loader.Next = chain;
            chain = loader;
        }

        return chain;
    }

    private static IEnumerable<string> ExpandPaths(IEnumerable<string> paths)
    {
        var lookup = paths.ToLookup(IsDirectory);
        var result = new List<string>();
        result.AddRange(lookup[false]);
        foreach (var path in lookup[true])
        {
            result.Add(path);
            result.AddRange(Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories));
        }
        return result;
    }

    private static IEnumerable<ITreeItem> ToHierarchy(IList<ITreeItem> flatNodes)
    {
        var nodesByPath = flatNodes.ToDictionary(node => node.Path);
        var groupsByParent = flatNodes.ToLookup(node => Path.GetDirectoryName(node.Path));

        foreach (var group in groupsByParent)
        {
            nodesByPath.TryGetValue(group.Key, out var parent);

            if (parent != null)
            {
                foreach (var item in group)
                {
                    item.Parent = parent;
                    parent.Children.Add(item);
                }
            }
        }

        return nodesByPath.Values.Where(x => x.Parent == null).ToList();
    }
}
