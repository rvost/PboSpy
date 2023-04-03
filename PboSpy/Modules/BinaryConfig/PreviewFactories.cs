using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;

namespace PboSpy.Modules.BinaryConfig;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".rvmat", ".sqm" })]
    public static Document PreviewDetectConfig(FileBase entry)
    {
        var text = entry.GetDetectConfigAsText(out _);
        return new TextPreviewViewModel(entry, text);
    }
}
