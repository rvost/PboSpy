namespace PboSpy.Modules.ConfigExplorer.ViewModels.Search;

public class SearchParams
{
    public SearchParams(string searchString, SearchMatch match) : this()
    {
        OriginalSearchString = SearchString = searchString ?? string.Empty;
        Match = match;
    }

    public SearchParams()
    {
        SearchString = string.Empty;
        Match = SearchMatch.StringIsContained;
        MinimalSearchStringLength = 1;
    }

    public string SearchString { get; private set; }

    public string OriginalSearchString { get; private set; }

    public SearchMatch Match { get; private set; }

    public bool IsSearchStringEmpty => string.IsNullOrEmpty(SearchString);

    public int MinimalSearchStringLength { get; set; }

    public int MatchSearchString(string stringToFind)
    {
        stringToFind ??= string.Empty;

        stringToFind = stringToFind.ToUpper();

        switch (Match)
        {
            case SearchMatch.StringIsContained:
                return stringToFind.IndexOf(SearchString);

            case SearchMatch.StringIsMatched:
                if (SearchString == stringToFind)
                    return 0;
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    string.Format("Internal Error: Search option '{0}' not implemented.", Match));
        }

        return -1;
    }

    public void SearchStringTrim()
    {
        SearchString = SearchString.Trim();
    }

    public void SearchStringToUpperCase()
    {
        SearchString = SearchString.ToUpper();
    }
}
