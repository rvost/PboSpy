using PboSpy.Interfaces;
using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.FileManager.Services;

internal class PhysicalFileLoader : FileLoader
{
    private HashSet<string> _supportedExtensions;

    public PhysicalFileLoader(IEnumerable<string> supportedExtensions)
    {
        _supportedExtensions = new(supportedExtensions);
    }

    public override ITreeItem Load(string path)
    {
        if (File.Exists(path) && _supportedExtensions.Contains(Path.GetExtension(path)))
        {
            return new PhysicalFile(Path.GetFullPath(path));
        }
        return base.Load(path);
    }
}

