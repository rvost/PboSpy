using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BIS.Core.Config;
using BIS.PAA;

namespace PboExplorer.Models;

public abstract class FileBase
{
    public abstract Stream GetStream();

    public abstract string Extension { get; }
    public abstract string Name { get; }
    public abstract string FullPath { get; }
    public abstract int DataSize { get; }

    public string GetText()
    {
        using var reader = new StreamReader(GetStream());
        return reader.ReadToEnd();
    }

    public string GetBinaryConfigAsText()
    {
        return GetBinaryConfig().ToString();

    }
    public ParamFile GetBinaryConfig()
    {
        using var stream = GetStream();
        return new ParamFile(stream);
    }

    public PAA GetPaaImage()
    {
        using var paaStream = GetStream();
        return new PAA(paaStream, Extension == ".pac");
    }

    public BitmapSource GetPaaAsBitmapSource()
    {
        using var paaStream = GetStream();
        var paa = new PAA(paaStream, Extension == ".pac");
        var pixels = PAA.GetARGB32PixelData(paa, paaStream);
        var colors = paa.Palette.Colors.Select(c => Color.FromRgb(c.R8, c.G8, c.B8)).ToList();
        var bitmapPalette = colors.Count > 0 ? new BitmapPalette(colors) : null;
        return BitmapSource.Create(paa.Width, paa.Height, 300, 300, PixelFormats.Bgra32,
            bitmapPalette, pixels, paa.Width * 4);
    }

    public string GetDetectConfigAsText(out bool wasBinary)
    {
        using var stream = GetStream();
        if (IsBinaryConfig(stream))
        {
            wasBinary = true;
            return new ParamFile(stream).ToString();
        }
        wasBinary = false;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static bool IsBinaryConfig(Stream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        stream.Seek(0, SeekOrigin.Begin);
        return buffer.SequenceEqual(new byte[] { 0, (byte)'r', (byte)'a', (byte)'P' });
    }

    public virtual bool IsBinaryConfig()
    {
        if (DataSize > 4)
        {
            using var stream = GetStream();
            return IsBinaryConfig(stream);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        return obj is FileBase @base &&
               FullPath == @base.FullPath;
    }
}
