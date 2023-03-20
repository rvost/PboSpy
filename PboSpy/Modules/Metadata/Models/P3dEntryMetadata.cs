using BIS.Core.Streams;
using PboSpy.Models;
using System.ComponentModel;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("P3D")]
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
        Type = p3d.IsEditable ? "MLOD" : "ODOL";
        BboxMax = p3d.ModelInfo.BboxMax.ToString();
        BboxMin = p3d.ModelInfo.BboxMin.ToString();
        MapType = p3d.ModelInfo.MapType.ToString();
        Class = p3d.ModelInfo.Class;
        Version = p3d.Version;
        Lods = p3d.LODs.Count();
    }
}
