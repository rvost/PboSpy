using PboExplorer.Models;

namespace PboExplorer.Interfaces;

public interface ITreeItemTransformer<T>
{
    T Transform(PboDirectory entry);
    T Transform(PboEntry entry);
    T Transform(PboFile entry);
    T Transform(PhysicalFile entry);
    T Transform(PhysicalDirectory entry);
    T Transform(ConfigClassItem entry);
}
