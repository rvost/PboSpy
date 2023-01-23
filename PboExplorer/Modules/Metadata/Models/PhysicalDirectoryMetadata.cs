using System.ComponentModel;
using System.IO;
using PboExplorer.Models;
using PboExplorer.Modules.Metadata.Utils;

namespace PboExplorer.Modules.Metadata.Models;

class PhysicalDirectoryMetadata : IMetadata
{
    [DisplayName("Full Path")]
    public string FullPath { get; set; }
    [DisplayName("Name")]
    public string Name { get; set; }
    public string Size { get; set; }
    [DisplayName("Created At")]
    public DateTime CreatedAt { get; set; }

    public PhysicalDirectoryMetadata(PhysicalDirectory file)
    {
        FullPath = file.FullPath;
        var info = new DirectoryInfo(file.FullPath);
        Name = info.Name;
        Size = Formatters.FormatSize(DirSize(info));
        CreatedAt = info.CreationTime;
    }

    private static double DirSize(DirectoryInfo d)
    {
        double size = 0;

        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }

        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }
        return size;
    }
}