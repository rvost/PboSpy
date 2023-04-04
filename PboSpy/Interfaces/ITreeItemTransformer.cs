using PboSpy.Models;

namespace PboSpy.Interfaces;

public interface ITreeItemTransformer<T>
{
    T Transform(PboDirectory entry);
    T Transform(PboEntry entry);
    T Transform(PboFile entry);
    T Transform(PhysicalFile entry);
    T Transform(PhysicalDirectory entry);
}
