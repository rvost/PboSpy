using System.Globalization;
using System.IO;
using System.Windows.Data;

using PboSpy.Helpers;
using PboSpy.Interfaces;

namespace PboSpy.Converters
{
    internal class TreeItemToIconConverter : IValueConverter
    {
        private readonly IIconService _iconService;
        private readonly Dictionary<string, string> _mapping = new()
        {
            {".bin", "bin" },
            {".cpp", "bin" },
            {".hpp", "bin" },
            {".p3d", "p3d" },
            {".paa", "paa" },
            {".pac", "paa" },
            {".pbo", "pbo" },
            {".rvmat", "rvmat" },
            {".sqf", "script" },
            {".sqm", "script" },
            {".wrp", "wrp" },
            {".bikey", "key" },
            {".bisign", "bisign" }
        };

        public TreeItemToIconConverter(IIconService iconService)
        {
            _iconService = iconService;
        }
        public TreeItemToIconConverter() : this(new IconService())
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ITreeItem treeItem = (ITreeItem)value;

            string extension = Path.GetExtension(treeItem.Name).ToLowerInvariant();

            if (!_mapping.TryGetValue(extension, out var iconKey))
            {
                iconKey = (treeItem.Children?.Any() ?? false) ? "FolderClosed" : "file";
            }

            return _iconService.GetIcon(iconKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
