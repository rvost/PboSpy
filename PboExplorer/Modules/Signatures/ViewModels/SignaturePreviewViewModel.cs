using Microsoft.Win32;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.ViewModels;
using PboExplorer.Modules.Signatures.Commands;
using System.IO;

namespace PboExplorer.Modules.Signatures.ViewModels;

internal class SignaturePreviewViewModel : PreviewViewModel, ICommandHandler<ExtractBiKeyCommandDefinition>
{
    public string Signature { get; }
    public BiKey BiKey { get; }
    public string Key => BiKey.ToString();

    public SignaturePreviewViewModel(FileBase model) : base(model)
    {
        using var stream = model.GetStream();

        BiKey = BiKey.ReadFromSignature(stream);

        using var ms = new MemoryStream();
        model.GetStream().CopyTo(ms);
        Signature = BitConverter.ToString(ms.ToArray()).Replace("-", ":");
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
            FileName = $"{BiKey.Authority}.bikey",
            Filter = "BiKey|*.bikey"
        };

        if (dlg.ShowDialog() == true)
        {
            using var output = File.OpenWrite(dlg.FileName);
            BiKey.WriteToStream(output);
        }

        return Task.CompletedTask;
    }
}
