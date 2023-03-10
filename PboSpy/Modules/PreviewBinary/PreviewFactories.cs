using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using PboSpy.Modules.PreviewBinary.ViewModels;

namespace PboSpy.Modules.PreviewBinary;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".rtm", ".wss", ".ogg", ".bin",
        ".fxy", ".wsi", ".shp", ".dbf", ".shx", ".bisurf" })]
    public static Document PreviewGenericBinary(FileBase entry)
    {
        if (entry.IsBinaryConfig())
        {
            var text = entry.GetBinaryConfigAsText();
            return new TextPreviewViewModel(entry, text);
        }
        else
        {
            return new BinaryPreviewViewModel(entry);
        }
    }

    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".rvmat", ".sqm" })]
    public static Document PreviewDetectConfig(FileBase entry)
    {
        var text = entry.GetDetectConfigAsText(out _);
        return new TextPreviewViewModel(entry, text);
    }
}
