using PboSpy.Models;
using PboSpy.Modules.Metadata;

namespace PboSpy.Modules.ConfigExplorer.Metadata;

[Export(typeof(IMetadataHandler))]
class ConfigItemMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is ConfigClassItem item)
        {
            var stringified = item.Properties.ToDictionary(k => k.Key, k => k.Value.ToString());

            return new ConfigClassMetadata(stringified, item.Name);
        }
        else
        {
            return Next?.Handle(obj, context);
        }
    }
}
