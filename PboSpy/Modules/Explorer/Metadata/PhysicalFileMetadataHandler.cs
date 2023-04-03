using PboSpy.Models;
using PboSpy.Modules.Metadata;

namespace PboSpy.Modules.Explorer.Metadata;

[Export(typeof(IMetadataHandler))]
class PhysicalFileMetadataHandler : MetadataHandler
{
    public override MetadataHandlerPriority Priority => MetadataHandlerPriority.FormatAgnostic;

    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PhysicalFile entry)
        {
            return new PhysicalFileMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
