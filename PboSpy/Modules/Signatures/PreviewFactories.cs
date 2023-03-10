using PboSpy.Models;
using PboSpy.Modules.Signatures.ViewModels;

namespace PboSpy.Modules.Signatures;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bisign" })]
    public static Document PreviewSignature(FileBase entry)
    {
        return new SignaturePreviewViewModel(entry);
    }

    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bikey" })]
    public static Document PreviewKey(FileBase entry)
    {
        return new KeyPreviewViewModel(entry);
    }
}
