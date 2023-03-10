using System.ComponentModel;
using PboSpy.Models;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Metadata.Models;

class PboDirectoryMeatdata : IMetadata
{
    [Description("Uncompressed size")]
    public string Size { get; set; }

    [DisplayName("Size In PBO")]
    [Description("Actual size in PBO file")]
    public string SizeInPbo { get; set; }

    public PboDirectoryMeatdata(PboDirectory directory)
    {
        Size = Formatters.FormatSize(directory.UncompressedSize);
        SizeInPbo = Formatters.FormatSize(directory.DataSize);
    }
}