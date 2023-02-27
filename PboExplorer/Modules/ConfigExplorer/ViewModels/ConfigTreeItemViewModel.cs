using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.Utils;
using PboExplorer.Modules.ConfigExplorer.ViewModels.Search;
using System.Windows.Data;

namespace PboExplorer.Modules.ConfigExplorer.ViewModels;

class ConfigTreeItemViewModel : PropertyChangedBase, IHasDummyChild
{
    private static readonly ConfigTreeItemViewModel DummyChild = new();

    private readonly ConfigClassItem _model;
    private readonly List<ConfigTreeItemViewModel> _backUpNodes = new();
    private readonly ObservableCollection<ConfigTreeItemViewModel> _children = new();

    private readonly object _itemsLock = new object();

    private bool _isExpanded = false;
    private MatchType _match = MatchType.NoMatch;
    private ISelectionRange _range = null;

    public ConfigTreeItemViewModel(ConfigClassItem model, ConfigTreeItemViewModel parent) : this()
    {
        _model = model;

        Id = model.Id;
        Parent = parent;
        Name = model.Name;

        ClearChildren(false);  // Lazy Load Children
    }

    protected ConfigTreeItemViewModel()
    {
        BindingOperations.EnableCollectionSynchronization(_children, _itemsLock);
    }

    public Guid Id { get; }

    public string Name { get; }

    // TODO: Improve encapsulation
    public ConfigClassItem Model => _model;

    public ConfigTreeItemViewModel Parent { get; private set; }

    public MatchType Match
    {
        get => _match;

        private set
        {
            if (_match != value)
            {
                _match = value;
                NotifyOfPropertyChange(() => Match);
            }
        }
    }

    public ISelectionRange Range
    {
        get => _range;

        // TODO: Simplify
        private set
        {
            if (_range != null && value != null)
            {
                // Nothing changed - so we change nothing here
                if (_range.Start == value.Start &&
                    _range.End == value.End &&
                    _range.SelectionBackground == value.SelectionBackground &&
                    _range.SelectionBackground == value.SelectionBackground)
                    return;

                _range = (ISelectionRange)value.Clone();
                NotifyOfPropertyChange(() => Range);
            }

            if (_range == null && value != null ||
                _range != null && value == null)
            {
                _range = (ISelectionRange)value.Clone();
                NotifyOfPropertyChange(() => Range);
            }
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }
    }

    public IEnumerable<ConfigTreeItemViewModel> BackUpNodes => _backUpNodes;

    public IEnumerable<ConfigTreeItemViewModel> Children => _children;

    public int ChildrenCount => _backUpNodes.Count;

    // TODO: Refactor
    public Uri IconSource
    {
        get
        {
            if (_model?.Children?.Count > 0)
            {
                return new("pack://application:,,,/PboExplorer;component/Resources/Icons/ClassCollection.png");
            }
            else
            {
                return new("pack://application:,,,/PboExplorer;component/Resources/Icons/Class.png");
            }
        }
    }

    public virtual bool HasDummyChild
        => _children?.Count == 1 && _children[0] == DummyChild;

    ///<summary>
    /// Convert a Model into a ViewModel using
    /// a LevelOrderTraversal Algorithm via TreeLib library.
    ///</summary>
    /// TODO: Refactor
    public static ConfigTreeItemViewModel GetViewModelFromModel(ConfigClassItem srcRoot)
    {
        if (srcRoot == null)
        {
            throw new ArgumentNullException(nameof(srcRoot));
        }

        // TODO: Remove cast
        var srcItems = TreeLib.BreadthFirst.Traverse.LevelOrder(srcRoot, item => item.Children.Cast<ConfigClassItem>());
        var IdToVmMap = new Dictionary<Guid, ConfigTreeItemViewModel>();

        ConfigTreeItemViewModel dstRoot = null;

        foreach (var node in srcItems.Select(i => i.Node))
        {
            if (node.Parent.Id == Guid.Empty)
            {
                dstRoot = new ConfigTreeItemViewModel(node, null);
                IdToVmMap.TryAdd(dstRoot.Id, dstRoot);
            }
            else
            {
                IdToVmMap.TryGetValue(node.Parent.Id, out var vmParent);

                var dstNode = new ConfigTreeItemViewModel(node, vmParent);
                vmParent.AddChild(dstNode);     // Insert converted ViewModel below ViewModel parent
                IdToVmMap.TryAdd(dstNode.Id, dstNode);
            }
        }
        
        // Destroy temp ID look-up structure
        IdToVmMap.Clear();

        return dstRoot;
    }

    public int LoadChildren()
    {
        ClearChildren(false, false);

        if (_backUpNodes.Count > 0)
        {
            _backUpNodes.Apply(item => AddChild(item, false));
        }

        return _children.Count;
    }

    public void ClearChildren(bool clearBackup = true, bool addDummyChild = true)
    {
        try
        {
            lock (_itemsLock)
            {
                _children.Clear();

                if (addDummyChild == true)
                    _children.Add(DummyChild);
            }

            if (clearBackup == true)
                _backUpNodes.Clear();
        }
        catch
        {
        }
    }

    public MatchType ProcessNodeMatch(SearchParams searchParams, out int matchStart)
    {
        matchStart = searchParams.MatchSearchString(Name);

        // Determine whether this node is a match or not
        var nodeMatch = matchStart >= 0 ? MatchType.NodeMatch : MatchType.NoMatch;

        ClearChildren(false);

        if (ChildrenCount > 0)
        {
            // Evaluate children by adding only thos children that match
            var maxChildMatch = MatchType.NoMatch;
            foreach (var item in BackUpNodes)
            {
                if (item.Match != MatchType.NoMatch)
                {
                    // Expand this item if it or one of its children contains a match
                    if (item.Match == MatchType.SubnodeMatch ||
                        item.Match == MatchType.Node_AND_SubnodeMatch)
                    {
                        item.SetExpand(true);
                    }
                    else
                        item.SetExpand(false);

                    if (maxChildMatch < item.Match)
                        maxChildMatch = item.Match;

                    AddChild(item, false);
                }
            }

            if (nodeMatch == MatchType.NoMatch && maxChildMatch != MatchType.NoMatch)
                nodeMatch = MatchType.SubnodeMatch;

            if (nodeMatch == MatchType.NodeMatch && maxChildMatch != MatchType.NoMatch)
                nodeMatch = MatchType.Node_AND_SubnodeMatch;
        }

        return nodeMatch;
    }

    public void SetExpand(bool isExpanded)
    {
        IsExpanded = isExpanded;
    }

    public MatchType SetMatch(MatchType match, int matchStart = -1, int matchEnd = -1)
    {
        Match = match;
        Range = new SelectionRange(matchStart, matchEnd);

        return match;
    }

    // TODO: Use non recursive implementation
    public void ClearMatch()
    {
        switch (Match)
        {
            case MatchType.NoMatch:
                break;
            case MatchType.NodeMatch:
                SetMatch(MatchType.NoMatch);
                break;
            case MatchType.SubnodeMatch:
            case MatchType.Node_AND_SubnodeMatch:
                _backUpNodes.Apply(node => node.ClearMatch());
                SetMatch(MatchType.NoMatch);
                break;
        }
    }

    private void AddChild(ConfigTreeItemViewModel child, bool addBackup = true)
    {
        try
        {
            if (HasDummyChild)
            {
                lock (_itemsLock)
                {
                    _children.Clear();
                }
            }

            lock (_itemsLock)
            {
                _children.Add(child);
            }

            if (addBackup)
            {
                _backUpNodes.Add(child);
            }
        }
        catch
        {
        }
    }

    private void RemoveChild(ConfigTreeItemViewModel child, bool removeBackup = true)
    {
        lock (_itemsLock)
        {
            _children.Remove(child);
        }

        if (removeBackup)
            _backUpNodes.Remove(child);
    }
}
