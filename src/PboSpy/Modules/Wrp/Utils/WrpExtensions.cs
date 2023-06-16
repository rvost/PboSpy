using BIS.WRP;
using PboSpy.Modules.Wrp.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Windows.Media.Imaging;

namespace PboSpy.Modules.PreviewWrp.Utils;

static class WrpExtensions
{
    public static BitmapSource PreviewElevation(this AnyWrp wrp)
    {
        var min = 4000d;

        var max = -1000d;

        for (int y = 0; y < wrp.TerrainRangeY; y++)
        {
            for (int x = 0; x < wrp.TerrainRangeX; x++)
            {
                max = Math.Max(wrp.Elevation[x + y * wrp.TerrainRangeY], max);
                min = Math.Min(wrp.Elevation[x + y * wrp.TerrainRangeY], min);
            }
        }

        var min0 = Math.Min(-1, min);
        min = Math.Max(0, min);
        var legend = new[]
        {
                new { E = min0, Color = SixLabors.ImageSharp.Color.DarkBlue.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min, Color = SixLabors.ImageSharp.Color.LightBlue.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.10, Color = SixLabors.ImageSharp.Color.DarkGreen.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.15, Color = SixLabors.ImageSharp.Color.Green.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.40, Color = SixLabors.ImageSharp.Color.Yellow.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.70, Color = SixLabors.ImageSharp.Color.Red.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = max, Color = SixLabors.ImageSharp.Color.Maroon.ToPixel<Rgb24>().ToScaledVector4() }
        };
        var img = new SixLabors.ImageSharp.Image<Bgra32>(wrp.TerrainRangeX, wrp.TerrainRangeY);
        for (int y = 0; y < wrp.TerrainRangeY; y++)
        {
            for (int x = 0; x < wrp.TerrainRangeX; x++)
            {
                var elevation = wrp.Elevation[x + y * wrp.TerrainRangeY];
                var before = legend.Where(e => e.E <= elevation).Last();
                var after = legend.FirstOrDefault(e => e.E > elevation) ?? legend.Last();
                var scale = (float)((elevation - before.E) / (after.E - before.E));
                Bgra32 rgb = new Bgra32();
                rgb.FromScaledVector4(Vector4.Lerp(before.Color, after.Color, scale));
                img[x, wrp.TerrainRangeY - y - 1] = rgb;
            }
        }
        return new ImageSharpImageSource(img);
    }
}
