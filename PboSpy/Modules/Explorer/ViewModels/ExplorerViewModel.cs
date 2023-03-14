using Microsoft.WindowsAPICodePack.Dialogs;
using PboSpy.Interfaces;
using PboSpy.Models;
using PboSpy.Modules.Metadata;
using PboSpy.Modules.PboManager;
using PboSpy.Modules.Preview;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PboSpy.Modules.Explorer.ViewModels;

[Export(typeof(IPboExplorer))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ExplorerViewModel : Tool, IPboExplorer
{
    private readonly IPboManager _pboManager;
    private readonly IPreviewManager _previewManager;
    private readonly IMetadataInspector _metadataInspector;

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
            NotifyOfPropertyChange(nameof(CanCloseSelected));
        }
    }
    public bool CanExtractSelectedPbo => SelectedItem is PboFile;

    public bool CanCloseSelected => SelectedItem is ITreeSubnode;

    public override PaneLocation PreferredLocation => PaneLocation.Left;

    [ImportingConstructor]
    public ExplorerViewModel(IPboManager pboManager, IPreviewManager previewManager, IMetadataInspector metadataInspector)
    {
        _pboManager = pboManager;
        _previewManager = previewManager;
        _metadataInspector = metadataInspector;

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

    public void CloseSelected()
    {
        if (SelectedItem is ITreeSubnode itemToClose)
        {
            _pboManager.Close(itemToClose);
        }

        SelectedItem = null;
    }

    public void CloseAll()
    {
        _pboManager.CloseAll();
        SelectedItem = null;
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

    public async void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            SelectedItem = item;
            await _metadataInspector.DispalyMetadataFor(item);
        }
    }

    public void OnDragOver(DragEventArgs e)
    {
        // Prevent drop of extracted files
        // Drop from Explorer has Copy|Move|Link effects
        // Drop from app has only Copy
        if (e.Effects == DragDropEffects.Copy)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }

    public void OnDrop(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            _pboManager.LoadSupportedFiles(paths);
        }
    }

    public void OnMoveItem(ITreeItem item, FrameworkElement sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (item is PboEntry entry)
            {
                var tempPath = GetTempFilePath(entry);
                var data = new DataObject();
                data.SetFileDropList(new StringCollection() { tempPath });
                DragDrop.DoDragDrop(sender, data, DragDropEffects.Copy);
            }
        }
    }

    private static string GetTempFilePath(PboEntry entry)
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), entry.Name);

        if (!File.Exists(tempFilePath))
        {
            entry.Extract(tempFilePath);
        }

        return tempFilePath;
    }
}
