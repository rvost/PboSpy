using PboExplorer.Models;

namespace PboExplorer.Modules.Preview;

public interface IPreviewManager
{
    Task ShowPreview(FileBase model);
}
