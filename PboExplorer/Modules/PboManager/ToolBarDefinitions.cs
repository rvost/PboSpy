using PboExplorer.Modules.PboManager.Commands;

namespace PboExplorer.Modules.PboManager;

internal static class ToolBarDefinitions
{
    [Export]
    public static readonly ToolBarDefinition PboManagerToolBar = new(0, "PboManager");

    [Export]
    public static readonly ToolBarItemGroupDefinition OpenToolBarGroup =
        new(PboManagerToolBar, 0);

    [Export]
    public static ToolBarItemDefinition OpenFilesToolBarItem =
        new CommandToolBarItemDefinition<OpenFilesCommandDefinition>(OpenToolBarGroup, 0);
    [Export]
    public static ToolBarItemDefinition OpenFolderToolBarItem =
        new CommandToolBarItemDefinition<OpenFolderCommandDefinition>(OpenToolBarGroup, 1);
}
