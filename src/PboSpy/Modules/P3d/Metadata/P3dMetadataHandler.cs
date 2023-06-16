using PboSpy.Modules.Metadata;
using PboSpy.Modules.Pbo.Models;

namespace PboSpy.Modules.P3d.Metadata;

[Export(typeof(IMetadataHandler))]
class P3dMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry && entry.Extension is ".p3d")
        {
            return new P3dEntryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
