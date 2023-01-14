using Gemini.Modules.Shell.Commands;
using Microsoft.Win32;

namespace PboExplorer.Modules.PboManager.Commands;

[CommandHandler]
public class OpenFilesCommandHandler : CommandHandlerBase<OpenFilesCommandDefinition>,
    ICommandHandler<OpenFileCommandDefinition>
{
    private readonly IPboManager _explorer;

    [ImportingConstructor]
    public OpenFilesCommandHandler(IPboManager explorer)
    {
        _explorer = explorer;
    }

    public override Task Run(Command command)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm",
            Multiselect = true
        };

        if (dlg.ShowDialog() == true)
        {
            _explorer.LoadSupportedFiles(dlg.FileNames);
        }

        return Task.CompletedTask;
    }
}
