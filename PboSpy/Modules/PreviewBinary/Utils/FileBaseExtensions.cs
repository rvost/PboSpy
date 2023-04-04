using BIS.Core.Config;
using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.PreviewBinary.Utils;

internal static class FileBaseExtensions
{
    public static string GetBinaryConfigAsText(this FileBase file)
    {
        return file.GetBinaryConfig().ToString();

    }

    public static ParamFile GetBinaryConfig(this FileBase file)
    {
        using var stream = file.GetStream();
        return new ParamFile(stream);
    }

    public static bool IsBinaryConfig(this FileBase file)
    {
        if (file.DataSize > 4)
        {
            using var stream = file.GetStream();
            return IsBinaryConfig(stream);
        }
        return false;
    }
    
    private static bool IsBinaryConfig(Stream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        stream.Seek(0, SeekOrigin.Begin);
        return buffer.SequenceEqual(new byte[] { 0, (byte)'r', (byte)'a', (byte)'P' });
    }
}
