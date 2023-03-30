using BIS.PBO;
using PboSpy.Models;
using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Pbo.Metadata;

[Export(typeof(IMetadataHandler))]
class PboEntryMetadataHandler : MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context)
    {
        if (obj is PboEntry entry)
        {
            context["Name"] = entry.Entry.FileName;
            context["Entry Name"] = entry.Entry.FileName;
            context["Entry Path"] = entry.FullPath;
            context["Timestamp"] = PBO.Epoch.AddSeconds(entry.Entry.TimeStamp);

            context["Is Compressed"] = entry.Entry.IsCompressed;
            context["Size"] = Formatters.FormatSize(entry.Entry.Size);
            context["SizeInPbo"] = Formatters.FormatSize(entry.Entry.DiskSize);
        }
        return base.Handle(obj, context);
    }
}
