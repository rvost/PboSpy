using BIS.Core.Streams;
using BIS.WRP;
using PboSpy.Models;
using System.ComponentModel;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("WRP")]
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
