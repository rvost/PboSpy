namespace PboExplorer.Modules.Explorer.Commands;

[CommandHandler]
public class ViewExplorerCommandHandler : CommandHandlerBase<ViewExplorerCommandDefinition>
{
    private readonly IShell _shell;

    [ImportingConstructor]
    public ViewExplorerCommandHandler(IShell shell)
    {
        _shell = shell;
    }

    public override Task Run(Command command)
    {
        _shell.ShowTool<IPboExplorer>();
        return Task.CompletedTask;
    }
}
