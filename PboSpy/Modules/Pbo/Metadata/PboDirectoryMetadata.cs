using PboSpy.Models;
using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;
using System.ComponentModel;

namespace PboSpy.Modules.Pbo.Metadata;

[DisplayName("PBO Directory")]
class PboDirectoryMetadata : IMetadata
{
    public string Name { get; set; }

    [Description("Uncompressed size")]
    public string Size { get; set; }

    [DisplayName("Size In PBO")]
    [Description("Actual size in PBO file")]
    public string SizeInPbo { get; set; }

    public PboDirectoryMetadata(PboDirectory directory)
    {
        Name = directory.Name;
        Size = Formatters.FormatSize(directory.UncompressedSize);
        SizeInPbo = Formatters.FormatSize(directory.DataSize);
    }
}
