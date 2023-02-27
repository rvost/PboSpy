using PboExplorer.Interfaces;

namespace PboExplorer.Modules.ConfigExplorer;

public interface IConfigManager
{
    ObservableCollection<ITreeItem> Items { get; }
}