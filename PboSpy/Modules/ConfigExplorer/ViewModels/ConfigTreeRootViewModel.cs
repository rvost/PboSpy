using PboSpy.Models;
using PboSpy.Modules.ConfigExplorer.Commands;
using PboSpy.Modules.ConfigExplorer.Utils;
using PboSpy.Modules.ConfigExplorer.ViewModels.Search;
using PboSpy.Modules.PboManager;
using System.Windows.Data;
using System.Windows.Input;

namespace PboSpy.Modules.ConfigExplorer.ViewModels;

[Export]
[PartCreationPolicy(CreationPolicy.Shared)]
class ConfigTreeRootViewModel : PropertyChangedBase, IDisposable
{
    private readonly IPboManager _pboManager;
    private readonly ConfigClassItem _configRoot = new();

    protected readonly ObservableCollection<ConfigTreeItemViewModel> _rootItems = new();
    protected readonly List<ConfigTreeItemViewModel> _backUpRoots = new();

    private readonly object _itemsLock = new();

    [ImportingConstructor]
    public ConfigTreeRootViewModel(IPboManager pboManager)
    {
        _pboManager = pboManager;

        BindingOperations.EnableCollectionSynchronization(_rootItems, _itemsLock);

        ExpandCommand = new RelayCommand<object>((p) =>
        {
            if (p is not ConfigTreeItemViewModel param)
                return;

            param.LoadChildren();
        });

        _pboManager.PboLoaded += OnPboLoaded;
        _pboManager.PboRemoved += OnPboRemoved;
    }

    public void Dispose()
    {
        _pboManager.PboLoaded -= OnPboLoaded;
        _pboManager.PboRemoved -= OnPboRemoved;
    }

    public ObservableCollection<ConfigTreeItemViewModel> RootItems => _rootItems;

    public int BackUpRootsCount => _backUpRoots.Count;

    public ICommand ExpandCommand { get; }

    /// <summary>
    /// Algorithm Source:
    /// https://blogs.msdn.microsoft.com/daveremy/2010/03/16/non-recursive-post-order-depth-first-traversal-in-c/
    /// </summary>
    public int DoSearch(SearchParams searchParams, CancellationToken token)
    {
        IList<ConfigTreeItemViewModel> backUpRoots = _backUpRoots;
        ObservableCollection<ConfigTreeItemViewModel> root = _rootItems;

        searchParams ??= new SearchParams();

        // TODO: Encapsulate
        searchParams.SearchStringTrim();
        searchParams.SearchStringToUpperCase();

        lock (_itemsLock)
        {
            root.Clear();
        }

        // Show all root items if string to search is empty
        if (searchParams.IsSearchStringEmpty == true ||
            searchParams.MinimalSearchStringLength >= searchParams.SearchString.Length)
        {
            foreach (var rootItem in backUpRoots)
            {
                if (token.IsCancellationRequested == true)
                    return 0;

                // Clear all highlighting
                rootItem.ClearMatch();
                rootItem.ClearChildren(false);
                rootItem.SetExpand(false);

                lock (_itemsLock)
                {
                    root.Add(rootItem);
                }
            }

            return 0;
        }

        int matchCount = 0;

        // Go through all root items and process their children
        foreach (var rootItem in backUpRoots)
        {
            if (token.IsCancellationRequested == true)
                return 0;

            rootItem.SetMatch(MatchType.NoMatch);

            // Match children of this root item
            var nodeMatchCount = MatchNodes(rootItem, searchParams);

            matchCount += nodeMatchCount;

            // Match this root item and find correct match type between
            // parent and children below
            int offset = -1;
            if ((offset = searchParams.MatchSearchString(rootItem.Name)) >= 0)
            {
                rootItem.SetMatch(MatchType.NodeMatch, offset, offset + searchParams.SearchString.Length);
            }

            if (nodeMatchCount > 0)
            {
                if (rootItem.Match == MatchType.NodeMatch)
                    rootItem.SetMatch(MatchType.Node_AND_SubnodeMatch, offset, offset + searchParams.SearchString.Length);
                else
                    rootItem.SetMatch(MatchType.SubnodeMatch);
            }

            // Determine wether root item should be visible and expanded or not
            if (rootItem.Match != MatchType.NoMatch)
            {
                if ((rootItem.Match & (MatchType.SubnodeMatch | MatchType.Node_AND_SubnodeMatch)) != 0)
                    rootItem.SetExpand(true);
                else
                    rootItem.SetExpand(false);

                lock (_itemsLock)
                {
                    root.Add(rootItem);
                }

            }
        }

        return matchCount;
    }

    private void AddItems(IEnumerable<ConfigClassItem> models)
    {
        foreach (var model in models)
        {
            lock (_itemsLock)
            {
                var vm = ConfigTreeItemViewModel.GetViewModelFromModel(model);
                RootsAdd(vm, false);
            }
        }
    }

    public void RemoveItem(ConfigClassItem model)
    {
        var vm = _backUpRoots.FirstOrDefault(x => x.Name == model.Name);

        if (vm != null)
        {
            lock (_itemsLock)
            {
                _backUpRoots.Remove(vm);
                _rootItems.Remove(vm);
            }
        }
    }

    private void Clear()
    {
        lock (_itemsLock)
        {
            _backUpRoots.Clear();
            _rootItems.Clear();
        }
    }

    protected void RootsAdd(ConfigTreeItemViewModel vmItem, bool addBackupItemOnly)
    {
        _backUpRoots.Add(vmItem);

        if (!addBackupItemOnly)
            _rootItems.Add(vmItem);
    }

    /// <summary>
    /// Implement a PostOrder matching algorithm with one root node
    /// and returns the number of matching children found.
    /// </summary>
    /// TODO: Refactor 
    private static int MatchNodes(ConfigTreeItemViewModel root, SearchParams searchParams)
    {
        var toVisit = new Stack<ConfigTreeItemViewModel>();
        var visitedAncestors = new Stack<ConfigTreeItemViewModel>();
        int MatchCount = 0;

        toVisit.Push(root);
        while (toVisit.Count > 0)
        {
            var node = toVisit.Peek();
            if (node.ChildrenCount > 0)
            {
                if (visitedAncestors.PeekOrDefault() != node)
                {
                    visitedAncestors.Push(node);
                    toVisit.PushReversed(node.BackUpNodes);
                    continue;
                }

                visitedAncestors.Pop();
            }

            // Process Node and count matches (if any)
            var match = node.ProcessNodeMatch(searchParams, out var matchStart);
            node.SetMatch(match, matchStart, matchStart + searchParams.SearchString.Length);

            if (node.Match == MatchType.NodeMatch)
            {
                MatchCount++;
            }

            node.SetExpand(node.Match == MatchType.SubnodeMatch || node.Match == MatchType.Node_AND_SubnodeMatch);

            toVisit.Pop();
        }

        return MatchCount;
    }

    // TODO: Refactor ConfigTree update in OnPboLoaded and OnPboRemoved
    private void OnPboLoaded(object sender, PboManagerEventArgs e)
    {
        var configClasses = _configRoot.MergePbo(e.File);

        Clear();
        AddItems(configClasses);
    }

    private void OnPboRemoved(object sender, PboManagerEventArgs e)
    {
        var configClasses = _configRoot.RemovePbo(e.File);

        Clear();
        AddItems(configClasses);
    }
}
