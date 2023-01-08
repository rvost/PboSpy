using PboExplorer.Modules.Core.Models;

namespace PboExplorer.Modules.Core.Factories;

static class EntryMetaDataFactory
{
    public static PboEntryMetadata Create(PboEntry entry)
    {
        PboEntryMetadata metadata = null;
        try
        {
            metadata = entry.Extension switch
            {
                ".paa" or ".pac" => new PaaEntryMetadata(entry),
                ".rvmat" or ".sqm" => new ConfigEntryMetadata(entry),
                ".wrp" => new WrpEntryMetadata(entry),
                ".p3d" => new P3dEntryMetadata(entry),
                _ => new PboEntryMetadata(entry),
            };
        }
        catch { }

        return metadata;
    }
}
