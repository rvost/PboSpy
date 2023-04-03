using BIS.PBO;
using PboSpy.Models;
using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Pbo.Metadata;

[Export(typeof(IMetadataHandler))]
class PboEntryMetadataHandler : MetadataHandler
{
    public override MetadataHandlerPriority Priority => MetadataHandlerPriority.FormatGeneric;

    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry)
        {
            return new PboEntryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
