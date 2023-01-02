using PboExplorer.Modules.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Factories;

static class EntryMetaDataFactory
{
    public static PboEntryMetadata Create(PboEntry entry)
    {
        return entry.Extension switch
        {
            ".paa" or ".pac" => new PaaEntryMetadata(entry),
            ".rvmat" or ".sqm" => new ConfigEntryMetadata(entry),
            ".wrp" => new WrpEntryMetadata(entry),
            ".p3d" => new P3dEntryMetadata(entry),
            _ => new PboEntryMetadata(entry),
        };
    }
}
