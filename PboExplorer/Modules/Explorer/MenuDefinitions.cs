using PboExplorer.Modules.Explorer.Commands;

namespace PboExplorer.Modules.Explorer;

internal static class MenuDefinitions
{
    [Export]
    public static readonly MenuItemDefinition ViewExplorerMenuItem = new CommandMenuItemDefinition<ViewExplorerCommandDefinition>(
      Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);
}
