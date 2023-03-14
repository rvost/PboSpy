using System.IO;
using System.Text;

namespace PboSpy.Modules.Signatures.Utils;

internal static class BinaryReaderExtensions
{
    public static (byte[], string) ReadCString(this BinaryReader reader)
    {
        var buffer = new List<byte>();

        while (reader.PeekChar() != 0)
        {
            var num = reader.ReadByte();
            buffer.Add(num);
        }
        reader.ReadByte();

        var strValue = Encoding.UTF8.GetString(buffer.ToArray());
        return (buffer.ToArray(), strValue);
    }

    public static void WriteCString(this BinaryWriter writer, string strValue)
    {
        writer.Write(Encoding.UTF8.GetBytes(strValue));
        writer.Write((byte)0);
    }
}