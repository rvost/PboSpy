using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using PboExplorer.Modules.Core.ViewModels;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ViewConfigCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.ConfigExplorer";

    public override string Name => CommandName;

    public override string Text => "Config Explorer";

    public override string ToolTip => "Config Explorer";

}

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
        _shell.ShowTool<ConfigViewModel>();
        return TaskUtility.Completed;
    }

}