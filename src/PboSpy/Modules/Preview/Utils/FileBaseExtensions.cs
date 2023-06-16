using PboSpy.Models;
using System.IO;

namespace PboSpy.Modules.Preview.Utils;

internal static class FileBaseExtensions
{
    public static string GetText(this FileBase file)
    {
        using var reader = new StreamReader(file.GetStream());
        return reader.ReadToEnd();
    }
}
