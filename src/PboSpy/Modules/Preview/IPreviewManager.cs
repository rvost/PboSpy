using PboSpy.Models;

namespace PboSpy.Modules.Preview;

public interface IPreviewManager
{
    Task ShowPreview(FileBase model);
}
