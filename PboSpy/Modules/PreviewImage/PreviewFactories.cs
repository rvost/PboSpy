using PboSpy.Models;
using PboSpy.Modules.PreviewImage.ViewModels;
using System.Windows.Media.Imaging;

namespace PboSpy.Modules.PreviewImage;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".jpg", ".jpeg", ".png" })]
    public static Document PreviewImage(FileBase entry)
    {
        using var stream = entry.GetStream();
        var image = BitmapFrame.Create(stream,
            BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        return new ImagePreviewViewModel(entry, image);
    }

    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".paa", ".pac" })]
    public static Document PreviewPAA(FileBase entry)
    {
        var image = entry.GetPaaAsBitmapSource();
        return new ImagePreviewViewModel(entry, image);
    }
}
