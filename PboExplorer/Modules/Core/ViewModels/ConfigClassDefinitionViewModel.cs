using PboExplorer.Modules.Core.Models;

namespace PboExplorer.Modules.Core.ViewModels;

class ConfigClassDefinitionViewModel
{
    private readonly PboEntry _pboEntry;

    public string EntryPath => _pboEntry.FullPath;
    public string PboPath => _pboEntry.PBO.PBOFilePath;

    public string Code { get; }

    public ConfigClassDefinitionViewModel(PboEntry pboEntry, string code)
    {
        _pboEntry = pboEntry;
        Code = code;
    }
}