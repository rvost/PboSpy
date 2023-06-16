using Gemini.Modules.Shell.Commands;
using Microsoft.Win32;
using PboSpy.Models;
using PboSpy.Modules.Preview.ViewModels;
using PboSpy.Modules.Signatures.Commands;
using PboSpy.Modules.Signatures.Models;
using PboSpy.Modules.Signatures.Utils;
using PboSpy.Modules.Signatures.Views;
using System.IO;
using WpfHexaEditor.Core;

namespace PboSpy.Modules.Signatures.ViewModels;

// TODO: Refactor 
internal class SignaturePreviewViewModel : PreviewViewModel, ICommandHandler<ExtractBiKeyCommandDefinition>,
    ICommandHandler<SaveFileCommandDefinition>, ICommandHandler<SaveFileAsCommandDefinition>,
    ICommandHandler<SaveBiKeyCommandDefinition>
{
    private readonly BiSign _signModel;
    private readonly BiKey _keyModel;

    private KeyViewModel _key;
    private IHexEditorView _view;

    public KeyViewModel Key
    {
        get => _key;
        private set
        {
            _key = value;
            NotifyOfPropertyChange(nameof(Key));
        }
    }

    public string FileName => _model.FullPath;

    public List<CustomBackgroundBlock> Highlighting { get; }

    public SignaturePreviewViewModel(FileBase model, BiSign signModel) : base(model)
    {
        _signModel = signModel;
        _keyModel = BiKey.FromSignature(_signModel);

        Highlighting = _signModel.GetHighlighting();
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
        => command.Enabled = Key is null;

    Task ICommandHandler<ExtractBiKeyCommandDefinition>.Run(Command command)
    {
        Key = new KeyViewModel(_keyModel);
        return Task.CompletedTask;
    }

    void ICommandHandler<SaveBiKeyCommandDefinition>.Update(Command command)
        => command.Enabled = Key is not null;

    Task ICommandHandler<SaveBiKeyCommandDefinition>.Run(Command command)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Extract",
            FileName = $"{_keyModel.Name}.bikey",
            Filter = "BiKey|*.bikey"
        };

        if (dlg.ShowDialog() == true)
        {
            var output = File.OpenWrite(dlg.FileName);
            _keyModel.WriteToStream(output, false);
        }

        return Task.CompletedTask;
    }

    void ICommandHandler<SaveFileCommandDefinition>.Update(Command command)
        => command.Enabled = true;

    Task ICommandHandler<SaveFileCommandDefinition>.Run(Command command)
    {
        try
        {
            _view.SubmitChanges();
        }
        catch (IOException)
        {
            //TODO: Log and report error to user
        }
        return Task.CompletedTask;
    }

    void ICommandHandler<SaveFileAsCommandDefinition>.Update(Command command)
        => command.Enabled = true;

    Task ICommandHandler<SaveFileAsCommandDefinition>.Run(Command command)
    {
        try
        {
            var dlg = new SaveFileDialog
            {
                Title = "Save As",
                FileName = $"{_model.Name}.bisign",
                Filter = "BiSign|*.bisign"
            };

            if (dlg.ShowDialog() == true)
            {
                _view.SubmitChanges(dlg.FileName);
            }
        }
        catch (IOException)
        {
            //TODO: Log and report error to user
        }
        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            Key?.Dispose();
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }

    protected override void OnViewLoaded(object view)
        => _view = (IHexEditorView)view;
}
