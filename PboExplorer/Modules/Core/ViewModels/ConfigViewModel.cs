using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using Gemini.Modules.StatusBar;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Factories;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.Services;
using System;
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
    private readonly IPboManager _pboManager;
    private readonly IPropertyGrid _propertyGrid;
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;

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
    public ConfigViewModel(IShell shell, IPboManager pboManager, IPropertyGrid propertyGrid,
        IStatusBar statusBar)
    {
        _shell = shell;
        _pboManager = pboManager;
        _propertyGrid = propertyGrid;
        _statusBar = statusBar;
        _documentFactory = new DocumentFactory(); // TODO: consider injection

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
                var document = DocumentFactory.CreatePreview(classItem);
                if (document != null)
                {
                    await _shell.OpenDocumentAsync(document);
                }
            }
            catch (Exception ex)
            {
                _statusBar.Items.Clear();
                _statusBar.AddItem($"ERROR: {ex.Message}", new GridLength(1, GridUnitType.Star));
                _statusBar.AddItem(classItem.Name, new GridLength(1, GridUnitType.Auto));
            }
        }
    }

    public void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            _propertyGrid.SelectedObject = item.Metadata;
        }
    }
}
