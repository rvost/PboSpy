using Gemini.Framework.Commands;
using Microsoft.WindowsAPICodePack.Dialogs;
using PboExplorer.Helpers;
using PboExplorer.Modules.Core.Services;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Commands;

[CommandHandler]
public class OpenFolderCommandHandler : CommandHandlerBase<OpenFolderCommandDefinition>
{
    private readonly IPboManager _explorer;

    [ImportingConstructor]
    public OpenFolderCommandHandler(IPboManager explorer)
    {
        _explorer = explorer;
    }

    public override Task Run(Command command)
    {
        var dialog = new CommonOpenFileDialog
        {
            Title = "Load PBO archives from a directory",
            IsFolderPicker = true
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            _explorer.LoadSupportedFiles(DirectoryExtensions.GetSupportedFiles(dialog.FileName));
        }

        return Task.CompletedTask;
    }
}