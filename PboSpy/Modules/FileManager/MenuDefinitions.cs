using PboSpy.Modules.FileManager.Commands;

namespace PboSpy.Modules.FileManager;

internal static class MenuDefinitions
{
    // File
    [Export]
    public static readonly MenuItemDefinition FileOpenMenuItem = new TextMenuItemDefinition(
        Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 0, "Open",
        new("pack://application:,,,/Gemini;component/Resources/Icons/Open.png"));

    [Export]
    public static readonly MenuItemGroupDefinition FileOpenCascadeGroup = new(FileOpenMenuItem, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFilesMenuItem = new CommandMenuItemDefinition<OpenFilesCommandDefinition>(
       FileOpenCascadeGroup, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFolderMenuItem = new CommandMenuItemDefinition<OpenFolderCommandDefinition>(
        FileOpenCascadeGroup, 1);
}
