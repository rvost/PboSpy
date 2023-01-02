using Gemini.Framework;
using Gemini.Framework.Commands;
using Microsoft.Win32;
using PboExplorer.Modules.Core.Commands;
using PboExplorer.Modules.Core.Models;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.ViewModels;

public abstract class PreviewViewModel : Document, ICommandHandler<ExtractCurrentCommandDefinition>
{
    protected FileBase _model;

    public PreviewViewModel(FileBase model)
    {
        _model = model;
        DisplayName = _model.Name;
    }

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
}
