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

    public override async Task Run(Command command)
    {
        var dialog = new CommonOpenFileDialog
        {
            Title = "Load PBO archives from a directory",
            IsFolderPicker = true
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            await _explorer.LoadSupportedFiles(dialog.FileNames);
        }
    }
}
