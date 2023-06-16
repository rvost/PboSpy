using PboSpy.Modules.Signatures.Commands;

namespace PboSpy.Modules.Signatures;

internal static class MenuDefinitions
{

    [Export]
    public static readonly MenuItemGroupDefinition KeysMenuGroup =
        new(Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 1);

    [Export]
    public static readonly MenuItemDefinition ExtractBiKeyMenuItem =
        new CommandMenuItemDefinition<ExtractBiKeyCommandDefinition>(KeysMenuGroup, 1);

    [Export]
    public static readonly MenuItemDefinition SaveBiKeyMenuItem =
        new CommandMenuItemDefinition<SaveBiKeyCommandDefinition>(KeysMenuGroup, 2);
}
