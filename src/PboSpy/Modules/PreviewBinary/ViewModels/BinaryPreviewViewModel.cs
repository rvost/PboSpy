using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using System.IO;

namespace PboSpy.Modules.PreviewBinary.ViewModels;

internal class BinaryPreviewViewModel : PreviewViewModel
{
    public Stream Data { get; }

    public BinaryPreviewViewModel(FileBase model) : base(model)
    {
        Data = model.GetStream();
    }

    protected override void CanExecuteCopy(Command command)
        => command.Enabled = false;

    protected override Task ExecuteCopy(Command command)
        => throw new NotImplementedException();

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            Data.Dispose();
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }
}
