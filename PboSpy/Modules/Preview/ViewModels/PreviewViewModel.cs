using Microsoft.Win32;
using PboSpy.Interfaces;
using PboSpy.Models;
using PboSpy.Modules.Extraction.Commands;
using PboSpy.Modules.Pbo.Models;

namespace PboSpy.Modules.Preview.ViewModels;

[Export]
public abstract class PreviewViewModel : Document, IModelWrapper<ITreeItem>,
    ICommandHandler<ExtractCurrentCommandDefinition>, ICommandHandler<CopyToClipboardCommandDefinition>
{
    protected FileBase _model;

    public FileBase Model => _model;

    ITreeItem IModelWrapper<ITreeItem>.Model => Model as ITreeItem;

    public PreviewViewModel(FileBase model)
    {
        _model = model;
        DisplayName = _model.Name;
    }

    protected abstract void CanExecuteCopy(Command command);
    protected abstract Task ExecuteCopy(Command command);

    void ICommandHandler<ExtractCurrentCommandDefinition>.Update(Command command)
    {
        command.Enabled = _model is PboEntry;
    }

    Task ICommandHandler<ExtractCurrentCommandDefinition>.Run(Command command)
    {
        if (_model is PboEntry entry)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Extract",
                FileName = entry.Name,
                Filter = "All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                entry.Extract(dlg.FileName);
            }
        }
        return Task.CompletedTask;
    }

    void ICommandHandler<CopyToClipboardCommandDefinition>.Update(Command command)
        => CanExecuteCopy(command);

    Task ICommandHandler<CopyToClipboardCommandDefinition>.Run(Command command)
        => ExecuteCopy(command);
}
