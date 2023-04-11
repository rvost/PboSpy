using BIS.PBO;
using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;
using PboSpy.Modules.Pbo.Models;
using System.ComponentModel;

namespace PboSpy.Modules.Pbo.Metadata;

// TODO: Remove inheritance hierarchy
[DisplayName("PBO Entry")]
class PboEntryMetadata : IMetadata
{
    [Category("General")]
    [DisplayName("Entry Name")]
    public string Name { get; set; }

    [Category("General")]
    [DisplayName("PBO file")]
    public string PboFile { get; set; }

    [Category("General")]
    [DisplayName("Entry Path")]
    public string EntryPath { get; set; }

    [Category("General")]
    [DisplayName("Created at")]
    public DateTime Timestamp { get; set; }

    [Category("General")]
    [DisplayName("Is Compressed")]
    public bool IsCompressed { get; set; }

    [Category("General")]
    public string Size { get; set; }

    [Category("General")]
    [DisplayName("Size in PBO")]
    public string SizeInPbo { get; set; }

    public PboEntryMetadata(PboEntry entry)
    {
        PboFile = entry.PBO.PBOFilePath;
        Name = entry.Entry.FileName;
        EntryPath = entry.FullPath;
        Timestamp = PBO.Epoch.AddSeconds(entry.Entry.TimeStamp);

        IsCompressed = entry.Entry.IsCompressed;
        Size = Formatters.FormatSize(entry.Entry.Size);
        SizeInPbo = Formatters.FormatSize(entry.Entry.DiskSize);
    }
}
