using PboSpy.Modules.BinaryConfig.Utils;
using PboSpy.Modules.Pbo.Metadata;
using PboSpy.Modules.Pbo.Models;
using System.ComponentModel;

namespace PboSpy.Modules.BinaryConfig.Metadata;

[DisplayName("Config file")]
class ConfigEntryMetadata : PboEntryMetadata
{
    [Category("General")]
    public string Format { get; set; }
    public ConfigEntryMetadata(PboEntry entry) : base(entry)
    {
        entry.GetDetectConfigAsText(out var wasBinary);
        Format = wasBinary ? "Binarized" : "Text";
    }
}
