namespace PboExplorer.Modules.ConfigExplorer.Commands;

[CommandDefinition]
public class ViewConfigCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.ConfigExplorer";

    public override string Name => CommandName;

    public override string Text => "Config Explorer";

    public override string ToolTip => "Config Explorer";

}
