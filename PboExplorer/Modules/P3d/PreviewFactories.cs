using BIS.Core.Streams;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.ViewModels;
using System.Text;

namespace PboExplorer.Modules.P3d;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".p3d" })]
    public static Document PreviewP3D(FileBase entry)
    {
        using var stream = entry.GetStream();
        var p3d = StreamHelper.Read<BIS.P3D.P3D>(stream);
        var sb = new StringBuilder();

        var p3dType = p3d.IsEditable ? "MLOD" : "ODOL";
        sb.AppendLine($"Type: {p3dType}");
        sb.AppendLine($"Bbox Max: {p3d.ModelInfo.BboxMax}");
        sb.AppendLine($"Bbox Min: {p3d.ModelInfo.BboxMin}");
        sb.AppendLine($"MapType: {p3d.ModelInfo.MapType}");
        sb.AppendLine($"CLass: {p3d.ModelInfo.Class}");
        sb.AppendLine($"Version: {p3d.Version}");
        sb.AppendLine($"LODs: {p3d.LODs.Count()}");

        foreach (var lod in p3d.LODs)
        {
            sb.AppendLine("---------------------------------------------------------------------------------------------------");
            sb.AppendLine($"LOD {lod.Resolution}");
            sb.AppendLine($"    {lod.FaceCount} Faces, {lod.VertexCount} Vertexes, {lod.GetModelHashId()}");
            sb.AppendLine($"    Named properties");
            foreach (var prop in lod.NamedProperties.OrderBy(p => p.Item1))
            {
                sb.AppendLine($"        {prop.Item1} = {prop.Item2}");
            }
            sb.AppendLine($"    Named selections");
            foreach (var prop in lod.NamedSelections.OrderBy(m => m.Name))
            {
                var mat = prop.Material;
                var tex = prop.Texture;
                if (!string.IsNullOrEmpty(mat) || !string.IsNullOrEmpty(tex))
                {
                    sb.AppendLine($"        {prop.Name} (material='{mat}' texture='{tex}')");
                }
                else
                {
                    sb.AppendLine($"        {prop.Name}");
                }
            }
            sb.AppendLine($"    Textures");
            foreach (var prop in lod.GetTextures().OrderBy(m => m))
            {
                sb.AppendLine($"        {prop}");
            }
            sb.AppendLine($"    Materials");
            foreach (var prop in lod.GetMaterials().OrderBy(m => m))
            {
                sb.AppendLine($"        {prop}");
            }
            sb.AppendLine();
        }
        var text = sb.ToString();
        return new TextPreviewViewModel(entry, text);
    }
}
