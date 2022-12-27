using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PboExplorer.Helpers;

internal static class DirectoryExtensions
{
    private static readonly HashSet<string> _supportedExtensions = new() { ".pbo", ".paa", ".rvmat", ".bin",
        ".pac", "*.p3d", "*.wrp", "*.sqm" };

    public static IEnumerable<string> GetSupportedFiles(string arg)
    {
        
        return Directory.EnumerateFiles(arg, "*.*", SearchOption.AllDirectories)
            .Where(path => _supportedExtensions.Contains(Path.GetExtension(path)));
    }
}
