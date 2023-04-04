using BIS.PAA;
using PboSpy.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PboSpy.Modules.Paa.Utils;

internal static class FileBaseExtensions
{
    public static BitmapSource GetPaaAsBitmapSource(this FileBase file)
    {
        using var paaStream = file.GetStream();
        var paa = new PAA(paaStream, file.Extension == ".pac");
        var pixels = PAA.GetARGB32PixelData(paa, paaStream);
        var colors = paa.Palette.Colors.Select(c => Color.FromRgb(c.R8, c.G8, c.B8)).ToList();
        var bitmapPalette = colors.Count > 0 ? new BitmapPalette(colors) : null;
        return BitmapSource.Create(paa.Width, paa.Height, 300, 300, PixelFormats.Bgra32,
            bitmapPalette, pixels, paa.Width * 4);
    }

    public static PAA GetPaaImage(this FileBase file)
    {
        using var paaStream = file.GetStream();
        return new PAA(paaStream, file.Extension == ".pac");
    }
}
