using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using PboSpy.Modules.Signatures.Models;

namespace PboSpy.Modules.Signatures.ViewModels;

internal class KeyPreviewViewModel : PreviewViewModel
{
    public string FileNamePath => _model.FullPath;
    public KeyViewModel Key { get; }

    public KeyPreviewViewModel(FileBase model) : base(model)
    {
        var keyModel = BiKey.ReadFromStream(model.GetStream());
        Key = new KeyViewModel(keyModel);
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if(close)
        {
            Key.Dispose();
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }

    protected override void CanExecuteCopy(Command command)
    {
        command.Enabled = false;
    }

    protected override Task ExecuteCopy(Command command)
    {
        throw new NotImplementedException();
    }

}
