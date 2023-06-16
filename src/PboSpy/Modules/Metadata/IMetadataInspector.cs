using PboSpy.Interfaces;

namespace PboSpy.Modules.Metadata;

public interface IMetadataInspector
{
    void Clear();
    Task DispalyMetadataFor(object item);
}
