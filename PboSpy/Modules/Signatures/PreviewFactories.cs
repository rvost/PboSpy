using PboSpy.Models;
using PboSpy.Modules.Signatures.Models;
using PboSpy.Modules.Signatures.ViewModels;

namespace PboSpy.Modules.Signatures;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bisign" })]
    public static Document PreviewSignature(FileBase entry)
    {
        using var stream = entry.GetStream();
        var signModel = BiSign.ReadFromStream(stream);
        
        return new SignaturePreviewViewModel(entry, signModel);
    }

    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bikey" })]
    public static Document PreviewKey(FileBase entry)
    {
        using var stream = entry.GetStream();
        var keyModel = BiKey.ReadFromStream(stream);
        var keyViewModel = new KeyViewModel(keyModel);
        
        return new KeyPreviewViewModel(entry, keyViewModel);
    }
}
