using Microsoft.VisualBasic.FileIO;
using PboSpy.Models;
using PboSpy.Modules.Csv.Utils;
using PboSpy.Modules.Csv.ViewModels;
using System.Data;

namespace PboSpy.Modules.Csv;
internal static class PreviewFactories
{
    [Export("FilePreviewFactory")]
    [ExportMetadata("Extensions", new[] { ".csv" })]
    public static Document PreviewCSV(FileBase entry)
    {
        var table = ReadCsvToDataTable(entry);
        return new CsvPreviewViewModel(entry, table);
    }

    private static DataTable ReadCsvToDataTable(FileBase entry)
    {
        var result = new DataTable();

        using var tfp = new TextFieldParser(entry.GetStream());

        tfp.SetDelimiters(",");

        tfp.ReadFields().Apply(x => result.Columns.Add(x.Trim()));
        tfp.ReadAllRows().Apply(row => result.Rows.Add(row));

        return result;
    }
}
