using BIS.P3D;
using System.ComponentModel;

namespace PboSpy.Modules.P3d.ViewModels;

[DisplayName("P3D Summary")]
internal class P3dSummary
{
    public P3dSummary(P3D p3d)
    {
        Type = p3d.IsEditable ? "MLOD" : "ODOL";
        BboxMax = p3d.ModelInfo.BboxMax.ToString();
        BboxMin = p3d.ModelInfo.BboxMin.ToString();
        MapType = p3d.ModelInfo.MapType.ToString();
        Class = p3d.ModelInfo.Class;
        Version = p3d.Version;
        Lods = p3d.LODs.Count();
    }

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
}
