using PboSpy.Models;
using PboSpy.Modules.Metadata;

namespace PboSpy.Modules.Explorer.Metadata;

[Export(typeof(IMetadataHandler))]
class PhysicalDirectoryMetadataHandler : MetadataHandler
{
    public override MetadataHandlerPriority Priority => MetadataHandlerPriority.FormatAgnostic;

    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PhysicalDirectory entry)
        {
            return new PhysicalDirectoryMetadata(entry);
        }
        return base.Handle(obj, context);
    }
}
