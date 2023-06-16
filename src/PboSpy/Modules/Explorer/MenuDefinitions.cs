using PboSpy.Modules.Explorer.Commands;

namespace PboSpy.Modules.Explorer;

internal static class MenuDefinitions
{
    [Export]
    public static readonly MenuItemDefinition ViewExplorerMenuItem = new CommandMenuItemDefinition<ViewExplorerCommandDefinition>(
      Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition FileCloseAllMenuItem = new CommandMenuItemDefinition<CloseAllFilesCommandDefinition>(
     Gemini.Modules.MainMenu.MenuDefinitions.FileCloseMenuGroup, 2);
}
