using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.Services;
using PboExplorer.Modules.Metadata;
using PboExplorer.Modules.PboManager;
using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.ViewModels;

// TODO: Remove duplication with ExplorerViewModel
[Export(typeof(IConfigExplorer))]
[PartCreationPolicy(CreationPolicy.Shared)]
internal class ConfigViewModel : Tool, IConfigExplorer
{
    private readonly IPboManager _pboManager;
    private readonly ConfigPreviewManager _previewManager;
    private readonly ITreeItemTransformer<Task<IMetadata>> _metadataTransformer;
    private readonly IPropertyGrid _propertyGrid;

    private ITreeItem _selectedItem;

    public ICollection<ITreeItem> Items { get => _pboManager.ConfigTree; }

    public ITreeItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            NotifyOfPropertyChange(nameof(SelectedItem));
        }
    }

    public override PaneLocation PreferredLocation => PaneLocation.Left;

    [ImportingConstructor]
    public ConfigViewModel(IPboManager pboManager, IPropertyGrid propertyGrid, ConfigPreviewManager previewManager,
         ITreeItemTransformer<Task<IMetadata>> metadataTransformer)
    {
        _pboManager = pboManager;
        _previewManager = previewManager;
        _metadataTransformer = metadataTransformer;
        _propertyGrid = propertyGrid;

        DisplayName = "Config";
    }

    public async Task OpenPreview(ITreeItem item)
    {
        if (item != SelectedItem)
        {
            return; // Handle bubbling
        }

        if (item is ConfigClassItem classItem)
        {
            await _previewManager.ShowPreviewAsync(classItem);
        }
    }

    public async void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            SelectedItem = item;
            _propertyGrid.SelectedObject = await item.Reduce(_metadataTransformer);
        }
    }
}
