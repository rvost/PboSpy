using PboSpy.Interfaces;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace PboSpy.Modules.Explorer.Converters;

internal class TreeItemToIconConverter : IValueConverter
{
    private readonly string _folderIcon = "pack://application:,,,/PboSpy;component/Resources/Icons/FolderClosed.png";
    private readonly string _fileIcon = "pack://application:,,,/PboSpy;component/Resources/Icons/file.png";

    private readonly Dictionary<string, string> _mapping = new()
    {
        {".bin", "pack://application:,,,/PboSpy;component/Resources/Icons/bin.png" },
        {".cpp", "pack://application:,,,/PboSpy;component/Resources/Icons/bin.png" },
        {".hpp", "pack://application:,,,/PboSpy;component/Resources/Icons/bin.png" },
        {".p3d", "pack://application:,,,/PboSpy;component/Resources/Icons/p3d.png" },
        {".paa", "pack://application:,,,/PboSpy;component/Resources/Icons/paa.png" },
        {".pac", "pack://application:,,,/PboSpy;component/Resources/Icons/paa.png" },
        {".pbo", "pack://application:,,,/PboSpy;component/Resources/Icons/pbo.png" },
        {".rvmat", "pack://application:,,,/PboSpy;component/Resources/Icons/rvmat.png" },
        {".sqf", "pack://application:,,,/PboSpy;component/Resources/Icons/script.png" },
        {".sqm", "pack://application:,,,/PboSpy;component/Resources/Icons/script.png" },
        {".wrp", "pack://application:,,,/PboSpy;component/Resources/Icons/wrp.png" },
        {".bikey", "pack://application:,,,/PboSpy;component/Resources/Icons/key.png" },
        {".bisign", "pack://application:,,,/PboSpy;component/Resources/Icons/bisign.png" }
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ITreeItem treeItem = (ITreeItem)value;

        var extension = Path.GetExtension(treeItem.Name).ToLowerInvariant();

        _mapping.TryGetValue(extension, out var iconKey);
        iconKey ??= treeItem.Children?.Any() ?? false ? _folderIcon : _fileIcon;
        
        return iconKey;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
