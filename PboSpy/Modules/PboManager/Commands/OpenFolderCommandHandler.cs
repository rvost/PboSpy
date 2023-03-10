using Microsoft.WindowsAPICodePack.Dialogs;

namespace PboSpy.Modules.PboManager.Commands;

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
            _explorer.LoadSupportedFiles(dialog.FileNames);
        }

        return Task.CompletedTask;
    }
}