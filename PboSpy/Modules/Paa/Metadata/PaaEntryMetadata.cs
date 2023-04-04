using PboSpy.Models;
using PboSpy.Modules.Paa.Utils;
using PboSpy.Modules.Pbo.Metadata;
using System.ComponentModel;

namespace PboSpy.Modules.Paa.Metadata;

[DisplayName("PAA")]
class PaaEntryMetadata : PboEntryMetadata
{
    [Category("PAA")]
    [DisplayName("Image Size")]
    public string ImageSize { get; set; }

    [Category("PAA")]
    [DisplayName("Image Type")]
    public string ImageType { get; set; }

    public PaaEntryMetadata(PboEntry entry) : base(entry)
    {
        var paa = entry.GetPaaImage();
        ImageSize = $"{paa.Width}x{paa.Height}";
        ImageType = paa.Type.ToString();
    }
}
