using PboExplorer.Interfaces;

namespace PboExplorer.Modules.ConfigExplorer;

public interface IConfigManager
{
    ICollection<ITreeItem> Items { get; }
}