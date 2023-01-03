using BIS.Core.Streams;
using BIS.WRP;
using Gemini.Framework;
using PboExplorer.Helpers;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PboExplorer.Modules.Core.Factories;

// TODO: Refactor
class DocumentFactory
{
    public Document CreatePreview(FileBase entry)
    {
        return entry.Extension switch
        {
            ".paa" or ".pac" => ShowPAA(entry),
            ".jpg" or ".jpeg" or ".png" => ShowImage(entry),
            ".rvmat" or ".sqm" => ShowDetectConfig(entry),
            ".wrp" => ShowWRP(entry),
            ".p3d" => ShowP3D(entry),
            ".rtm" or ".wss" or ".ogg" or ".bin" or ".fxy" or ".wsi" or
            ".shp" or ".dbf" or ".shx" or ".bisurf" => ShowGenericBinary(entry),
            _ => ShowGenericText(entry),
        };
    }

    public Document CreatePreview(ConfigClassItem entry)
        => new ConfigClassViewModel(entry);

    private Document ShowGenericText(FileBase entry)
    {
        var text = entry.GetText();
        return new TextPreviewViewModel(entry, text);
    }

    private Document ShowGenericBinary(FileBase entry)
    {
        if (entry.IsBinaryConfig())
        {
            var text = entry.GetBinaryConfigAsText();
            return new TextPreviewViewModel(entry, text);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private Document ShowP3D(FileBase entry)
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

    private Document ShowWRP(FileBase entry)
    {
        using var stream = entry.GetStream();
        var wrp = StreamHelper.Read<AnyWrp>(stream);
        var image = wrp.PreviewElevation();
        return new ImagePreviewViewModel(entry, image);
    }

    private Document ShowDetectConfig(FileBase entry)
    {
        var text = entry.GetDetectConfigAsText(out _);
        return new TextPreviewViewModel(entry, text);
    }

    private Document ShowImage(FileBase entry)
    {
        using var stream = entry.GetStream();
        var image = BitmapFrame.Create(stream,
            BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        return new ImagePreviewViewModel(entry, image);
    }

    private Document ShowPAA(FileBase entry)
    {
        var paa = entry.GetPaaImage();
        var image = paa.Bitmap;
        return new ImagePreviewViewModel(entry, image);
    }
}
