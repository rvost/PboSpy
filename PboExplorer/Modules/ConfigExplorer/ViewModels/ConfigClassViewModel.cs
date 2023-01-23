using PboExplorer.Interfaces;
using PboExplorer.Models;

namespace PboExplorer.Modules.ConfigExplorer.ViewModels;

class ConfigClassViewModel : Document, IModelWrapper<ITreeItem>
{
    private readonly ConfigClassItem _model;

    public ITreeItem Model => _model;

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
