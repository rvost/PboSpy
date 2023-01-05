using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Modules.PropertyGrid;
using PboExplorer.Modules.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ViewExplorerCommandDefinition: CommandDefinition
{
    public const string CommandName = "View.PboExplorer";

    public override string Name => CommandName;

    public override string Text => "Pbo Explorer";

    public override string ToolTip => "Pbo Explorer";
 
}

[CommandHandler]
public class ViewExplorerCommandHandler: CommandHandlerBase<ViewExplorerCommandDefinition>
{
    private readonly IShell _shell;

    [ImportingConstructor]
    public ViewExplorerCommandHandler(IShell shell)
    {
        _shell = shell;
    }

    public override Task Run(Command command)
    {
        _shell.ShowTool<ExplorerViewModel>();
        return TaskUtility.Completed;
    }
}
