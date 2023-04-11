using Gemini.Modules.Shell.Commands;
using Microsoft.Win32;

namespace PboSpy.Modules.FileManager.Commands;

[CommandHandler]
public class OpenFilesCommandHandler : CommandHandlerBase<OpenFilesCommandDefinition>,
    ICommandHandler<OpenFileCommandDefinition>
{
    private readonly IFileManager _explorer;

    [ImportingConstructor]
    public OpenFilesCommandHandler(IFileManager explorer)
    {
        _explorer = explorer;
    }

    public override async Task Run(Command command)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm;*.bisign;*.bikey",
            Multiselect = true
        };

        if (dlg.ShowDialog() == true)
        {
            await _explorer.LoadSupportedFiles(dlg.FileNames);
        }

    }
}
