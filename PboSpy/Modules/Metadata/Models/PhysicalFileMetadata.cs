using System.ComponentModel;
using System.IO;
using PboSpy.Models;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("Physical File")]
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
