using Gemini.Framework.Commands;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ExtractAsTextCommandDefinition: CommandDefinition
{
    public const string CommandName = "Edit.ExtractAsText";

    public override string Name => CommandName;

    public override string Text => "Extract As Text";

    public override string ToolTip => "Extract current file as text";
}
