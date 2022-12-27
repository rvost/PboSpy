namespace PboExplorer.Helpers;

internal static class Formatters
{
    private static readonly string[] _sizes = { "Bytes", "KiB", "MiB", "GiB", "TiB" };

    public static string FormatSize(double size)
    {
        
        int order = 0;
        while (size >= 1024 && order < _sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return string.Format("{0:0.##} {1}", size, _sizes[order]);
    }
}
