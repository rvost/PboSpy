using System.ComponentModel;
using PboExplorer.Models;

namespace PboExplorer.Modules.Metadata.Models;

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