using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.ConfigExplorer.Utils;

internal static class FileBaseExtensions
{
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
