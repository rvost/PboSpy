using PboSpy.Modules.Signatures.Models;
using System.IO;

namespace PboSpy.Modules.Signatures.ViewModels;

class KeyViewModel : IDisposable
{
    private readonly BiKey _biKey;
    private readonly Stream _data;

    public string Authority => _biKey.Name;
    public Stream Data => _data;

    public KeyViewModel(BiKey biKey)
    {
        _biKey = biKey;
        _data = new MemoryStream();
        _biKey.WriteToStream(_data, true);
        _data.Seek(0, SeekOrigin.Begin);
    }

    public void Dispose()
    {
        _data.Close();
    }
}
