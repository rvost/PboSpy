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
}
