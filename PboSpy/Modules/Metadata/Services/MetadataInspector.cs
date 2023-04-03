using Gemini.Modules.PropertyGrid;
using Microsoft.Extensions.Logging;
using PboSpy.Interfaces;

namespace PboSpy.Modules.Metadata.Services;

[Export(typeof(IMetadataInspector))]
internal class MetadataInspector : IMetadataInspector
{
    private readonly IPropertyGrid _propertyGrid;
    private readonly IMetadataHandler _handlerChain;
    private readonly ILogger _logger;

    [ImportingConstructor]
    public MetadataInspector([ImportMany(typeof(IMetadataHandler))] IEnumerable<Lazy<IMetadataHandler>> handlerExports,
        IPropertyGrid propertyGrid, ILoggerFactory loggerFactory)
    {
        _handlerChain = BuildHandlerChain(handlerExports);
        _propertyGrid = propertyGrid;
        _logger = loggerFactory.CreateLogger<MetadataInspector>();
    }

    public void Clear()
        => _propertyGrid.SelectedObject = null;

    public async Task DispalyMetadataFor(ITreeItem item)
    {
        try
        {
            _propertyGrid.SelectedObject = await Task.Run(() => _handlerChain.Handle(item, new()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying metadata for {Item}", item);
        }
    }

    private static IMetadataHandler BuildHandlerChain(IEnumerable<Lazy<IMetadataHandler>> exports)
    {
        var handlers = exports
            .Select(o => o.Value)
            .OrderBy(h => (int)h.Priority);

        IMetadataHandler chain = new CatchAllMetadataHandler();
        foreach (var handler in handlers)
        {
            handler.Next = chain;
            chain = handler;
        }

        return chain;
    }
}
