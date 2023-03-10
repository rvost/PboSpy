using Microsoft.Win32;
using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using PboSpy.Modules.Signatures.Commands;
using PboSpy.Modules.Signatures.Models;
using System.IO;

namespace PboSpy.Modules.Signatures.ViewModels;

internal class SignaturePreviewViewModel : PreviewViewModel, ICommandHandler<ExtractBiKeyCommandDefinition>
{
    private readonly BiKey _keyModel;

    public KeyViewModel Key { get; private set; }
    public string FilePath => _model.FullPath;

    public SignaturePreviewViewModel(FileBase model) : base(model)
    {
        using var stream = model.GetStream();
        _keyModel = BiKey.ReadFromSignature(stream);
        Key = new KeyViewModel(_keyModel);
    }

    protected override void CanExecuteCopy(Command command)
    {
        command.Enabled = false;
    }

    protected override Task ExecuteCopy(Command command)
    {
        throw new NotImplementedException();
    }

    void ICommandHandler<ExtractBiKeyCommandDefinition>.Update(Command command)
    {
        command.Enabled = true;
    }

    Task ICommandHandler<ExtractBiKeyCommandDefinition>.Run(Command command)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Extract",
            FileName = $"{_keyModel.Authority}.bikey",
            Filter = "BiKey|*.bikey"
        };

        if (dlg.ShowDialog() == true)
        {
            var output = File.OpenWrite(dlg.FileName);
            _keyModel.WriteToStream(output, false);
        }

        return Task.CompletedTask;
    }
}
