using Gemini.Modules.PropertyGrid;
using Microsoft.WindowsAPICodePack.Dialogs;
using PboExplorer.Interfaces;
using PboExplorer.Models;
using PboExplorer.Modules.PboManager;
using PboExplorer.Modules.Preview;
using System.Windows;

namespace PboExplorer.Modules.Explorer.ViewModels;

[Export(typeof(IPboExplorer))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ExplorerViewModel : Tool, IPboExplorer
{
    private readonly IPboManager _pboManager;
    private readonly IPreviewManager _previewManager;
    private readonly IPropertyGrid _propertyGrid;

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
    public ExplorerViewModel(IPboManager pboManager, IPropertyGrid propertyGrid, IPreviewManager previewManager)
    {
        _pboManager = pboManager;
        _previewManager = previewManager;
        _propertyGrid = propertyGrid;

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
            await _previewManager.ShowPreview(file);
        }
    }

    public void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            SelectedItem = item;
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
