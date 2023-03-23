using PboSpy.Modules.Metadata.Utils;
using System.ComponentModel;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("Config Class")]
class ConfigClassMetadata : DictionaryPropertyGridAdapter<string, string>, IMetadata
{
    public string Name { get; set; }

    public ConfigClassMetadata(Dictionary<string, string> dict, string name) : base(dict)
    {
        Name = name;
    }
}
