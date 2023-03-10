using System.ComponentModel;
using PboSpy.Models;

namespace PboSpy.Modules.Metadata.Models;

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
