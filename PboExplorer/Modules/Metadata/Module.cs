using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Modules.ConfigExplorer.ViewModels;
using PboExplorer.Modules.Preview.ViewModels;

namespace PboExplorer.Modules.Metadata;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IPropertyGrid _propertyGrid;
    

    [ImportingConstructor]
    public Module(IPropertyGrid propertyGrid)
    {
        _propertyGrid = propertyGrid;
    }

    public override void Initialize()
    {
        Shell.ActiveDocumentChanged += (sender, e) => RefreshPropertyGrid();
        RefreshPropertyGrid();
    }

    private void RefreshPropertyGrid()
    {
        _propertyGrid.SelectedObject = Shell.ActiveItem switch
        {
            PreviewViewModel preview => (preview.Model as ITreeItem)?.Metadata, // TODO: Refactor PreviewViewModel
            ConfigClassViewModel config => config.Model?.Metadata,
            _ => null,
        };
    }
}
