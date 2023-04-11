using BIS.PBO;
using PboSpy.Interfaces;
using PboSpy.Models;
using PboSpy.Modules.FileManager;
using System.IO;

namespace PboSpy.Modules.Pbo;

[Export(typeof(IFileLoader))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class PboLoader : FileLoader
{
    public override ITreeItem Load(string path)
    {
        if (string.Equals(Path.GetExtension(path), ".pbo", StringComparison.OrdinalIgnoreCase))
        {
            return new PboFile(new PBO(path, false));
        }
        return base.Load(path);
    }
}

