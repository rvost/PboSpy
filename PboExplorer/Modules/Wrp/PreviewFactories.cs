using BIS.Core.Streams;
using BIS.WRP;
using PboExplorer.Models;
using PboExplorer.Modules.PreviewImage.ViewModels;
using PboExplorer.Modules.PreviewWrp.Utils;

namespace PboExplorer.Modules.PreviewWrp;

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
