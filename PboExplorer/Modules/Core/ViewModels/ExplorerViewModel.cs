using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using Microsoft.WindowsAPICodePack.Dialogs;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Factories;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.Services;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Move to separate module
[Export]
public class ExplorerViewModel : Tool
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

    public ICollection<ITreeItem> Items { get => PboManager.FileTree; }

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

    public ExplorerViewModel()
    {
        _documentFactory = new DocumentFactory();
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
        if (item is FileBase file)
        {
            try
            {
                var document = _documentFactory.CreatePreview(file);
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

    // TODO: Refactor
    public void OnDrop(DragEventArgs args)
    {
        if (args.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])args.Data.GetData(DataFormats.FileDrop);

            // Split folders and files
            var lookup = paths.ToLookup(
                (path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                );

            // Load files from folders
            lookup[true].ToList().ForEach(
                dir => PboManager.LoadSupportedFiles(DirectoryExtensions.GetSupportedFiles(dir))
                );

            // Load other files
            PboManager.LoadSupportedFiles(lookup[false]);
        }
    }
}
