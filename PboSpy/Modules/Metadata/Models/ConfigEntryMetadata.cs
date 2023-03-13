using System.ComponentModel;
using PboSpy.Models;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("Config file")]
class ConfigEntryMetadata : PboEntryMetadata
{
    [Category("General")]
    public string Format { get; set; }
    public ConfigEntryMetadata(PboEntry entry) : base(entry)
    {
        entry.GetDetectConfigAsText(out bool wasBinary);
        Format = wasBinary ? "Binarized" : "Text";
    }
}