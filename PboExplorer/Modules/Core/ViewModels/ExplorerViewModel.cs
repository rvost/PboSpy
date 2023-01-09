using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using Gemini.Modules.StatusBar;
using Microsoft.WindowsAPICodePack.Dialogs;
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

// TODO: Move to separate module
[Export]
public class ExplorerViewModel : Tool
{
    private readonly DocumentFactory _documentFactory;
    private readonly IPboManager _pboManager;
    private readonly IPropertyGrid _propertyGrid;
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;

    private ITreeItem _selectedItem;

    public ICollection<ITreeItem> Items { get => _pboManager.FileTree; }

    public ITreeItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            NotifyOfPropertyChange(nameof(SelectedItem));
            NotifyOfPropertyChange(nameof(CanExtractSelectedPbo));
        }
    }
    public bool CanExtractSelectedPbo
    {
        get => SelectedItem is PboFile;
    }

    public override PaneLocation PreferredLocation => PaneLocation.Left;

    [ImportingConstructor]
    public ExplorerViewModel(IShell shell, IPboManager pboManager, IPropertyGrid propertyGrid,
        IStatusBar statusBar)
    {
        _shell = shell;
        _pboManager = pboManager;
        _propertyGrid = propertyGrid;
        _statusBar = statusBar;
        _documentFactory = new DocumentFactory(); // TODO: consider injection

        DisplayName = "PBO Explorer";
    }

    public void ExtractSelectedPbo()
    {
        if (SelectedItem is PboFile selectedPbo)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Extract to",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPbo.Extract(dialog.FileName);
            }
        }
    }

    public async Task OpenPreview(ITreeItem item)
    {
        if (item != SelectedItem)
        {
            return; // Handle bubbling
        }

        if (item is FileBase file)
        {
            try
            {
                var document = _documentFactory.CreatePreview(file);
                if (document != null)
                {
                    await _shell.OpenDocumentAsync(document);
                }
            }
            catch (Exception ex) 
            {
                _statusBar.Items.Clear();
                _statusBar.AddItem($"ERROR: {ex.Message}", new GridLength(1, GridUnitType.Star));
                _statusBar.AddItem(file.Name, new GridLength(1, GridUnitType.Auto));
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

    public void OnDrop(DragEventArgs args)
    {
        if (args.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])args.Data.GetData(DataFormats.FileDrop);

            _pboManager.LoadSupportedFiles(paths);
        }
    }
}
