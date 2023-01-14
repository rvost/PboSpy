using System.Windows;
using System.Windows.Media;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace PboExplorer.Helpers
{
    interface IIconService
    {
        ImageSource GetIcon(string iconKey);
    }
    internal class IconService : IIconService
    {
        private readonly Dictionary<string, ImageSource> _cache = new();

        public ImageSource GetIcon(string iconKey)
        {
            if (!_cache.TryGetValue(iconKey, out var imageSource))
            {
                var uri = GetUriFromKey(iconKey);
                var resourceStream = Application.GetResourceStream(uri);

                var reader = new FileSvgReader(new WpfDrawingSettings { TextAsGeometry = false, IncludeRuntime = true });
                var drawing = reader.Read(resourceStream.Stream);

                imageSource = new DrawingImage(drawing);
                _cache[iconKey] = imageSource;
            }

            return imageSource;
        }

        private Uri GetUriFromKey(string iconKey)
            => new($"/Resources/Icons/{iconKey}.svg", UriKind.Relative);
    }
}
