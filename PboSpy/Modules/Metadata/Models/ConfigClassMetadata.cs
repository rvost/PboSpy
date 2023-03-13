using System.ComponentModel;
using PboSpy.Modules.Metadata.Utils;

namespace PboSpy.Modules.Metadata.Models;

[DisplayName("Config Class")]
class ConfigClassMetadata : DictionaryPropertyGridAdapter<string, string>
{
    public string Name { get; set; }

    public ConfigClassMetadata(Dictionary<string, string> dict, string name) : base(dict)
    {
        Name = name;
    }
}