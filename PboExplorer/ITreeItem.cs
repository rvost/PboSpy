using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer
{
    public interface ITreeItem
    {
        string Name { get; }

        ICollection<ITreeItem> Children { get; }
    }
}
