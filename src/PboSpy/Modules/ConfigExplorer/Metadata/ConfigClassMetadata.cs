using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;
using System.ComponentModel;

namespace PboSpy.Modules.ConfigExplorer.Metadata;

[DisplayName("Config Class")]
class ConfigClassMetadata : DictionaryPropertyGridAdapter<string, string>, IMetadata
{
    public string Name { get; set; }

    public ConfigClassMetadata(Dictionary<string, string> dict, string name) : base(dict)
    {
        Name = name;
    }
}
