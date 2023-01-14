namespace PboExplorer.Modules.ConfigExplorer.Commands;

[CommandHandler]
public class ViewConfigCommandHandler : CommandHandlerBase<ViewConfigCommandDefinition>
{
    private readonly IShell _shell;

    [ImportingConstructor]
    public ViewConfigCommandHandler(IShell shell)
    {
        _shell = shell;
    }

    public override Task Run(Command command)
    {
        _shell.ShowTool<IConfigExplorer>();
        return Task.CompletedTask;
    }

}