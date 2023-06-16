using PboSpy.Modules.Metadata;
using PboSpy.Modules.Pbo.Models;

namespace PboSpy.Modules.Pbo.Metadata;

[Export(typeof(IMetadataHandler))]
class PboDirectoryMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboDirectory entry)
        {
            return new PboDirectoryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
