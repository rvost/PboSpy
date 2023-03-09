using BIS.Core.Streams;
using BIS.P3D;
using PboExplorer.Models;
using PboExplorer.Modules.P3d.ViewModels;

namespace PboExplorer.Modules.P3d;

internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".p3d" })]
    public static Document PreviewP3D(FileBase entry)
    {
        using var stream = entry.GetStream();
        var p3d = StreamHelper.Read<P3D>(stream);
        
        return new P3dPreviewViewModel(entry, p3d);
    }
}
