using PboSpy.Modules.Extraction.Commands;

namespace PboSpy.Modules.Extraction;

internal static class MenuDefinitions
{
    // Edit
    [Export]
    public static readonly MenuItemGroupDefinition EditMenuGroup = new(
        Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ExtractCurrentMenuItem = new CommandMenuItemDefinition<ExtractCurrentCommandDefinition>(
       EditMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition CopyToClipboardMenuItem = new CommandMenuItemDefinition<CopyToClipboardCommandDefinition>(
       EditMenuGroup, 1);

    [Export]
    public static readonly MenuItemGroupDefinition EditExtractAsMenuGroup = new(
       Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ExtractAsTextMenuItem = new CommandMenuItemDefinition<ExtractAsTextCommandDefinition>(
       EditExtractAsMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition ExtractAsPngMenuItem = new CommandMenuItemDefinition<ExtractAsPngCommandDefinition>(
      EditExtractAsMenuGroup, 1);
}
