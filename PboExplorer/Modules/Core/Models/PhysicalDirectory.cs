using System.Collections.Generic;
using System.Collections.ObjectModel;
using PboExplorer.Interfaces;

namespace PboExplorer.Modules.Core.Models;

class PhysicalDirectory : ITreeItem
{
    private readonly ObservableCollection<ITreeItem> files = new ObservableCollection<ITreeItem>();

    public string Name { get; }

    public PhysicalDirectory(string name)
    {
        Name = name;
    }

    public string FullPath { get; }

    public ICollection<ITreeItem> Children
    {
        get { return files; }
    }

    public IMetadata Metadata => null;
}
