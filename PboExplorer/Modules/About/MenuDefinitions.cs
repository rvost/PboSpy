using PboExplorer.Modules.About.Commands;

namespace PboExplorer.Modules.About;

internal static class MenuDefinitions
{
    [Export]
    public static readonly MenuItemGroupDefinition ViewMenuGroup = new(
       Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ViewAboutMenuItem = new CommandMenuItemDefinition<ViewAboutCommandDefinition>(
        ViewMenuGroup, 0);
}
