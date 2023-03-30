using PboSpy.Models;
using PboSpy.Modules.Metadata;

namespace PboSpy.Modules.Pbo.Metadata;

[Export(typeof(IMetadataHandler))]
class PboMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboFile entry)
        {
            return new PboMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
