using PboExplorer.Modules.ConfigExplorer.Commands;

namespace PboExplorer.Modules.ConfigExplorer;

internal static class MenuDefinitions
{
    [Export]
    public static readonly MenuItemDefinition ViewConfigMenuItem = new CommandMenuItemDefinition<ViewConfigCommandDefinition>(
      Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);
}
