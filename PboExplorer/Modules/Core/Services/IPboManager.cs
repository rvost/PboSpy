using System.Collections.Generic;

namespace PboExplorer.Modules.Core.Services;

public interface IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    void LoadSupportedFiles(IEnumerable<string> fileNames);
}
