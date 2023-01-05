using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Factories;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.Services;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Remove duplication with ExplorerViewModel
[Export]
public class ConfigViewModel : Tool
{
    private readonly DocumentFactory _documentFactory;
    private ITreeItem _selectedItem;

    // TODO: Use Constructor DI
    [Import]
    public IShell Shell { get; set; }
    // TODO: Use Constructor DI
    [Import]
    public IPboManager PboManager { get; set; }
    // TODO: Use Constructor DI
    [Import]
    public IPropertyGrid PropertyGrid { get; set; }

    public ICollection<ITreeItem> Items { get => PboManager.ConfigTree; }

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

    public ConfigViewModel()
    {
        _documentFactory = new DocumentFactory();
        DisplayName = "Config";
    }

    public async Task OpenPreview(ITreeItem item)
    {
        if(item != SelectedItem)
        {
            return; // Handle bubbling
        }

        if (item is ConfigClassItem classItem)
        {
            try
            {
                var document = _documentFactory.CreatePreview(classItem);
                if (document != null)
                {
                    await Shell.OpenDocumentAsync(document);
                }
            }
            catch
            {
                //TODO: Show error in status bar
            }
        }
    }

    public void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            PropertyGrid.SelectedObject = item.Metadata;
        }
    }
}
