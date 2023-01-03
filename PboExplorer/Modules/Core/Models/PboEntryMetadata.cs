using System;
using System.ComponentModel;
using BIS.Core.Streams;
using BIS.PBO;
using BIS.WRP;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;
using System.Linq;

namespace PboExplorer.Modules.Core.Models;

class PboEntryMetadata : IMetadata
{
    [Category("General")]
    [DisplayName("PBO file")]
    public string PboFile { get; set; }

    [Category("General")]
    [DisplayName("Entry Name")]
    public string EntryName { get; set; }

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
        EntryName = entry.Entry.FileName;
        EntryPath = entry.FullPath;
        Timestamp = PBO.Epoch.AddSeconds(entry.Entry.TimeStamp);

        IsCompressed = entry.Entry.IsCompressed;
        Size = Formatters.FormatSize(entry.Entry.Size);
        SizeInPbo = Formatters.FormatSize(entry.Entry.DiskSize);
    }
}

class PaaEntryMetadata : PboEntryMetadata
{
    [Category("PAA")]
    [DisplayName("Image Size")]
    public string ImageSize { get; set; }

    [Category("PAA")]
    [DisplayName("Image Type")]
    public string ImageType { get; set; }

    public PaaEntryMetadata(PboEntry entry) : base(entry)
    {
        var paa = entry.GetPaaImage();
        ImageSize = $"{paa.Width}x{paa.Height}";
        ImageType = paa.Type.ToString();
    }
}

class WrpEntryMetadata : PboEntryMetadata
{
    [Category("WRP")]
    [DisplayName("Cell Size")]
    public float CellSize { get; set; }

    [Category("WRP")]
    [DisplayName("Land Range")]
    public string LandRange { get; set; }

    [Category("WRP")]
    [DisplayName("Terrain Range")]
    public string TerrainRange { get; set; }

    [Category("WRP")]
    [DisplayName("Objects Count")]
    public int ObjectsCount { get; set; }

    [Category("WRP")]
    [DisplayName("Materials Count")]
    public int MaterialsCount { get; set; }

    public WrpEntryMetadata(PboEntry entry) : base(entry)
    {
        using var stream = entry.GetStream();
        var wrp = StreamHelper.Read<AnyWrp>(stream);
        CellSize = wrp.CellSize;
        LandRange = $"{wrp.LandRangeX}x{wrp.LandRangeY}";
        TerrainRange = $"{wrp.TerrainRangeX}x{wrp.TerrainRangeY}";
        ObjectsCount = wrp.ObjectsCount;
        MaterialsCount = wrp.MatNames.Length;
    }
}

class P3dEntryMetadata : PboEntryMetadata
{
    [Category("P3D")]
    public string Type { get; set; }
    [Category("P3D")]
    [DisplayName("B.Box Max")]
    [Description("Maximum coordinates of bounding box")]
    public string BboxMax { get; set; }
    [Category("P3D")]
    [DisplayName("B.Box Max")]
    [Description("Minimum coordinates of bounding box")]
    public string BboxMin { get; set; }
    [Category("P3D")]
    [DisplayName("Map Type")]
    public string MapType { get; set; }
    [Category("P3D")]
    public string Class { get; set; }
    [Category("P3D")]
    public int Version { get; set; }
    [Category("P3D")]
    [DisplayName("LODs Count")]
    public int Lods { get; set; }

    public P3dEntryMetadata(PboEntry entry) : base(entry)
    {
        using var stream = entry.GetStream();
        var p3d = StreamHelper.Read<BIS.P3D.P3D>(stream);
        this.Type = p3d.IsEditable ? "MLOD" : "ODOL";
        BboxMax = p3d.ModelInfo.BboxMax.ToString();
        BboxMin = p3d.ModelInfo.BboxMin.ToString();
        MapType = p3d.ModelInfo.MapType.ToString();
        Class = p3d.ModelInfo.Class;
        Version = p3d.Version;
        Lods = p3d.LODs.Count();
    }
}

class ConfigEntryMetadata : PboEntryMetadata
{
    [Category("General")]
    public string Format { get; set; }
    public ConfigEntryMetadata(PboEntry entry) : base(entry)
    {
        entry.GetDetectConfigAsText(out bool wasBinary);
        Format = wasBinary ? "Binarized" : "Text";
    }
}