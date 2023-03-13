using Gemini.Modules.PropertyGrid;
using Microsoft.Extensions.Logging;
using PboSpy.Interfaces;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Metadata.Services;

[Export(typeof(IMetadataInspector))]
internal class MetadataInspector : IMetadataInspector
{
    private readonly IPropertyGrid _propertyGrid;
    private readonly MetadataTransformer _metadataTransformer;

    [ImportingConstructor]
    public MetadataInspector(IPropertyGrid propertyGrid, ILoggerFactory loggerFactory)
    {
        _propertyGrid = propertyGrid;
        _metadataTransformer = new MetadataTransformer(loggerFactory);
    }

    public void Clear()
        => _propertyGrid.SelectedObject = null;

    public async Task DispalyMetadataFor(ITreeItem item)
        => _propertyGrid.SelectedObject = await item?.Reduce(_metadataTransformer);
}
