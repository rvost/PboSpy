using Microsoft.VisualBasic.FileIO;

namespace PboSpy.Modules.Csv.Utils;

internal static class TextFieldParserExtensions
{
    public static IEnumerable<string[]> ReadAllRows(this TextFieldParser reader)
    {
        while (!reader.EndOfData)
        {
            yield return reader.ReadFields();
        }
    }
}
