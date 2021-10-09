using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer
{
    public abstract class BasicTreeItem
    {
        public BasicTreeItem(string name)
        {
            Name = name;
            Children = new List<BasicTreeItem>();
        }

        public BasicTreeItem(string name, IEnumerable<BasicTreeItem> children)
        {
            Name = name;
            Children = new List<BasicTreeItem>(children);
        }

        public string Name { get; }

        public List<BasicTreeItem> Children { get; }
    }
}
