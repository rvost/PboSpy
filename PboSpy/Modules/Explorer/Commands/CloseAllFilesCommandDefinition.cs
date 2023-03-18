namespace PboSpy.Modules.Explorer.Commands;

[CommandDefinition]
internal class CloseAllFilesCommandDefinition: CommandDefinition
{
    public override string Name => "File.CloseAllFiles";

    public override string Text => "Close All";

    public override string ToolTip => "Close all loaded files";

    public override Uri IconSource
       => new("pack://application:,,,/PboSpy;component/Resources/Icons/CloseAll.png");
}
