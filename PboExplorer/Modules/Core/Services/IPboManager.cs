using System.Collections.Generic;
using PboExplorer.Interfaces;

namespace PboExplorer.Modules.Core.Services;

public interface IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    public ICollection<ITreeItem> ConfigTree { get; }

    void LoadSupportedFiles(IEnumerable<string> fileNames);
}
