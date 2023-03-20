namespace PboSpy.Interfaces;

// TODO: Refactor design, find better name
public interface ITreeSubnode : ITreeItem
{
    ITreeItem Parent { get; }
}
