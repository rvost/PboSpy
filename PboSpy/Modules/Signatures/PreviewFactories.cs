using PboSpy.Models;
using PboSpy.Modules.Signatures.Models;
using PboSpy.Modules.Signatures.ViewModels;
using System.IO;

namespace PboSpy.Modules.Signatures;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".bisign" })]
    public static Document PreviewSignature(FileBase entry)
    {
        using var stream = entry.GetStream();
        var signModel = BiSign.ReadFromStream(stream);
        
        var memoryStream= new MemoryStream();
        signModel.WriteToStream(memoryStream, true);
        memoryStream.Position = 0;
        
        return new SignaturePreviewViewModel(entry, signModel, memoryStream);
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
