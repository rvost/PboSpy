using PboSpy.Models;
using PboSpy.Modules.Metadata;

namespace PboSpy.Modules.Wrp.Metadata;

[Export(typeof(IMetadataHandler))]
class WrpMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry && entry.Extension is ".wrp")
        {
            return new WrpEntryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
