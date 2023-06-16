namespace PboSpy.Modules.Signatures.Commands;

[CommandDefinition]
public class SaveBiKeyCommandDefinition : CommandDefinition
{
    public const string CommandName = "Edit.SaveBiKey";

    public override string Name => CommandName;

    public override string Text => "Save .bikey";

    public override string ToolTip => "Save .bikey from signature";

    public override Uri IconSource
        => new("pack://application:,,,/PboSpy;component/Resources/Icons/SaveKey.png");
}