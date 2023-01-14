using PboExplorer.Models;
using PboExplorer.Modules.Signatures.ViewModels;

namespace PboExplorer.Modules.Signatures;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bisign" })]
    public static Document PreviewSignature(FileBase entry)
    {
        return new SignaturePreviewViewModel(entry);
    }
}
