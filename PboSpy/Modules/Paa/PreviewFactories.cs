using PboSpy.Models;
using PboSpy.Modules.PreviewImage.ViewModels;

namespace PboSpy.Modules.Paa;
internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".paa", ".pac" })]
    public static Document PreviewPAA(FileBase entry)
    {
        var image = entry.GetPaaAsBitmapSource();
        return new ImagePreviewViewModel(entry, image);
    }
}
