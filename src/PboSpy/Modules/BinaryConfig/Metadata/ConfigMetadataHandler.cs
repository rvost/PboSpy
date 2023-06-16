using PboSpy.Modules.Metadata;
using PboSpy.Modules.Pbo.Models;

namespace PboSpy.Modules.BinaryConfig.Metadata;

[Export(typeof(IMetadataHandler))]
class ConfigMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry && entry.Extension is ".rvmat" or ".sqm")
        {
            return new ConfigEntryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
