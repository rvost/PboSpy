using System.ComponentModel;
using System.IO;
using PboExplorer.Models;
using PboExplorer.Modules.Metadata.Utils;

namespace PboExplorer.Modules.Metadata.Models;

class PhysicalFileMetadata : IMetadata
{
    [DisplayName("Full Path")]
    public string FullPath { get; set; }
    [DisplayName("File Name")]
    public string Name { get; set; }
    public string Size { get; set; }
    [DisplayName("Created At")]
    public DateTime CreatedAt { get; set; }

    public PhysicalFileMetadata(PhysicalFile file)
    {
        FullPath = file.FullPath;
        var info = new FileInfo(file.FullPath);
        Name = info.Name;
        Size = Formatters.FormatSize(info.Length);
        CreatedAt = info.CreationTime;
    }
}
