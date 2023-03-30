using PboSpy.Modules.Metadata.Utils;
using System.ComponentModel;

namespace PboSpy.Modules.Metadata;

[DisplayName("Metadata")]
internal class GenericMetadata : DictionaryPropertyGridAdapter<string, object>, IMetadata
{
    public GenericMetadata(Dictionary<string, object> dict) : base(dict) { }
}
