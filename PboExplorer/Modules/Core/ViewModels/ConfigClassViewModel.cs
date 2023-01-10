using Gemini.Framework;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace PboExplorer.Modules.Core.ViewModels;

class ConfigClassViewModel : Document
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
