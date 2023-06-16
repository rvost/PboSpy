using PboSpy.Interfaces;
using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.FileManager.Services;

internal class PhysicalDirectoryLoader : FileLoader
{
    public override ITreeItem Load(string path)
    {
        if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
        {
            var dirInfo = new DirectoryInfo(path);
            return new PhysicalDirectory(dirInfo.Name, Path.GetFullPath(path));
        }
        return base.Load(path);
    }
}

