using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;

namespace PboSpy.Modules.Signatures.ViewModels;

internal class KeyPreviewViewModel : PreviewViewModel
{
    public KeyViewModel Key { get; }

    public KeyPreviewViewModel(FileBase model, KeyViewModel keyViewModel) : base(model)
    {
        Key = keyViewModel;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            Key.Dispose();
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }

    protected override void CanExecuteCopy(Command command)
        => command.Enabled = false;

    protected override Task ExecuteCopy(Command command)
        => throw new NotImplementedException();

}
