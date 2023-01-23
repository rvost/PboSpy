namespace PboExplorer.Interfaces;

public interface ITreeItem
{
    string Name { get; }

    ICollection<ITreeItem> Children { get; }

    T Reduce<T>(ITreeItemTransformer<T> transformer);
}
