namespace PboSpy.Modules.Explorer.Commands;

[CommandDefinition]
public class ViewExplorerCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.PboExplorer";

    public override string Name => CommandName;

    public override string Text => "Pbo Explorer";

    public override string ToolTip => "Pbo Explorer";

}
