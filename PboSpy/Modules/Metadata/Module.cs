using PboSpy.Interfaces;

namespace PboSpy.Modules.Metadata;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IMetadataInspector _metadataInspector;

    [ImportingConstructor]
    public Module(IMetadataInspector metadataInspector)
    {
        _metadataInspector = metadataInspector;
    }

    public override void Initialize()
    {
        Shell.ActiveDocumentChanged += (sender, e) => RefreshPropertyGrid();
        RefreshPropertyGrid();
    }

    private async void RefreshPropertyGrid()
    {
        if (Shell.ActiveItem is IModelWrapper<ITreeItem> treeItemWrapper)
        {
            await _metadataInspector.DispalyMetadataFor(treeItemWrapper.Model);
        }
        else
        {
            _metadataInspector.Clear();
        }
    }
}
