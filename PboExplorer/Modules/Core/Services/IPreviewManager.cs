using PboExplorer.Modules.Core.Models;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Services;

public interface IPreviewManager
{
    Task ShowPreviewAsync(FileBase model);
    Task ShowPreviewAsync(ConfigClassItem model);
}
