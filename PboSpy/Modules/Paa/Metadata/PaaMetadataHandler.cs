using PboSpy.Modules.Metadata;
using PboSpy.Modules.Pbo.Models;

namespace PboSpy.Modules.Paa.Metadata;

[Export(typeof(IMetadataHandler))]
class PaaMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry && entry.Extension is ".paa" or ".pac")
        {
            return new PaaEntryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
