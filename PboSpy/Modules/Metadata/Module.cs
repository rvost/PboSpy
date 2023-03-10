using Gemini.Modules.PropertyGrid;
using PboSpy.Interfaces;

namespace PboSpy.Modules.Metadata;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IPropertyGrid _propertyGrid;
    private readonly ITreeItemTransformer<Task<IMetadata>> _metadataTransformer;

    [ImportingConstructor]
    public Module(IPropertyGrid propertyGrid, ITreeItemTransformer<Task<IMetadata>> metadataTransformer)
    {
        _propertyGrid = propertyGrid;
        _metadataTransformer = metadataTransformer;
    }

    public override void Initialize()
    {
        Shell.ActiveDocumentChanged += (sender, e) => RefreshPropertyGrid();
        RefreshPropertyGrid();
    }

    private async void RefreshPropertyGrid()
    {
        if(Shell.ActiveItem is IModelWrapper<ITreeItem> treeItemWrapper)
        {
            _propertyGrid.SelectedObject = await treeItemWrapper.Model?.Reduce(_metadataTransformer);
        }
        else
        {
            _propertyGrid.SelectedObject = null;
        }
    }
}
