using BIS.Core.Config;
using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.BinaryConfig.Utils;

internal static class FileBaseExtensions
{
    public static string GetDetectConfigAsText(this FileBase file, out bool wasBinary)
    {
        using var stream = file.GetStream();
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
}
