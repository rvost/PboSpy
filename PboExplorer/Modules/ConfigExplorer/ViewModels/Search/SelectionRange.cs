using System.Windows.Media;

namespace PboExplorer.Modules.ConfigExplorer.ViewModels.Search;

public class SelectionRange : PropertyChangedBase, ISelectionRange
{
    private int _start = 1;
    private int _end = 1;

    public SelectionRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public SelectionRange(SelectionRange copyThis)
    {
        if (copyThis == null)
            return;

        Start = copyThis.Start;
        End = copyThis.End;
        SelectionBackground = copyThis.SelectionBackground;
        NormalBackground = copyThis.NormalBackground;
        DarkSkin = copyThis.DarkSkin;
    }

    public int Start
    {
        get => _start;

        private set
        {
            if (_start != value)
            {
                _start = value;
                NotifyOfPropertyChange(() => Start);
            }
        }
    }


    public int End
    {
        get => _end;

        private set
        {
            if (_end != value)
            {
                _end = value;
                NotifyOfPropertyChange(() => End);
            }
        }
    }

    public bool DarkSkin { get; private set; } = false;

    /// <summary>
    /// Gets the background color that is applied to the background brush,
    /// which should be applied when no match is indicated
    /// (this can be default(Color) in which case standard selection Brush
    /// is applied).
    /// </summary>
    public Color SelectionBackground { get; set; } = default;

    /// <summary>
    /// Gets the background color that is applied to the background brush.
    /// which should be applied when no match is indicated
    /// (this can be default(Color) in which case Transparent is applied).
    /// </summary>
    public Color NormalBackground { get; set; } = default;

    public object Clone() => new SelectionRange(this);
}
