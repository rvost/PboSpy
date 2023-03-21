using Microsoft.Win32;
using PboSpy.Models;
using PboSpy.Modules.Extraction.Commands;
using PboSpy.Modules.Preview.ViewModels;
using System.Data;
using System.IO;
using System.Windows;

namespace PboSpy.Modules.Csv.ViewModels;
internal class CsvPreviewViewModel : PreviewViewModel, ICommandHandler<ExtractAsTextCommandDefinition>
{
    public DataTable Table { get; }
    public CsvPreviewViewModel(FileBase model, DataTable table) : base(model)
    {
        Table = table;
    }

    protected override void CanExecuteCopy(Command command)
    {
        command.Enabled = false;
    }

    protected override Task ExecuteCopy(Command command)
    {
        throw new NotImplementedException();
    }

    void ICommandHandler<ExtractAsTextCommandDefinition>.Update(Command command)
        => command.Enabled = true;

    async Task ICommandHandler<ExtractAsTextCommandDefinition>.Run(Command command)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Extract to text file",
            FileName = _model.Name,
            DefaultExt = ".csv",
            Filter = "CSV|*.csv|Text file|*.txt"
        };

        if (dlg.ShowDialog() == true)
        {
            using var r = new StreamReader(_model.GetStream());
            var text = await r.ReadToEndAsync();
            await File.WriteAllTextAsync(dlg.FileName, text);
        }
    }
}
