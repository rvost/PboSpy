using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Interfaces
{
    public interface ITreeItem
    {
        string Name { get; }

        ICollection<ITreeItem> Children { get; }

        IMetadata Metadata { get; }
    }
}
