using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using PreviewFunc = System.Func<PboSpy.Models.FileBase, Gemini.Framework.Document>;

namespace PboSpy.Modules.Preview.Factories;

[Export]
public class PreviewFactory
{
    private readonly Dictionary<string, PreviewFunc> _factories = new();

    [ImportingConstructor]
    public PreviewFactory([ImportMany("FilePreviewFactory")] IEnumerable<Lazy<PreviewFunc, IPreviewMetadata>> factories)
    {
        foreach (var f in factories)
        {
            f.Metadata.Extensions.Apply(ext => _factories.Add(ext, f.Value));
        }
    }

    public Document CreatePreview(FileBase entry)
    {
        if(_factories.TryGetValue(entry.Extension, out var factory))
        {
            return factory(entry);
        }
        else
        {
            return PreviewGenericText(entry);
        }
    }

    public static Document PreviewGenericText(FileBase entry)
    {
        var text = entry.GetText();
        return new TextPreviewViewModel(entry, text);
    }
}
