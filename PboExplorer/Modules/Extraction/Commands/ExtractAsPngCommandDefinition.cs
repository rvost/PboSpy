namespace PboExplorer.Modules.Extraction.Commands;

[CommandDefinition]
public class ExtractAsPngCommandDefinition : CommandDefinition
{
    public const string CommandName = "Edit.ExtractAsPng";

    public override string Name => CommandName;

    public override string Text => "Extract As PNG";

    public override string ToolTip => "Extract current file as image";

    public override Uri IconSource
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/ExtractImage.png");
}