using PboSpy.Modules.ConfigExplorer.Models;

namespace PboSpy.Modules.ConfigExplorer.ViewModels;

class ConfigClassViewModel : Document
{
    private readonly ConfigClassItem _model;

    public ICollection<ConfigClassDefinitionViewModel> Definitions { get; }

    public ConfigClassViewModel(ConfigClassItem model)
    {
        _model = model;
        DisplayName = _model.Name;

        Definitions = _model.Definitions
            .Select(kvp => new ConfigClassDefinitionViewModel(kvp.Key, kvp.Value.ToString(0)))
            .ToList();
    }
}
