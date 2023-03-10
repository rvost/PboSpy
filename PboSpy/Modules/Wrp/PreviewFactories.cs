using BIS.Core.Streams;
using BIS.WRP;
using PboSpy.Models;
using PboSpy.Modules.PreviewImage.ViewModels;
using PboSpy.Modules.PreviewWrp.Utils;

namespace PboSpy.Modules.PreviewWrp;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".wrp" })]
    public static Document PreviewWRP(FileBase entry)
    {
        using var stream = entry.GetStream();
        var wrp = StreamHelper.Read<AnyWrp>(stream);
        var image = wrp.PreviewElevation();
        return new ImagePreviewViewModel(entry, image);
    }
}
