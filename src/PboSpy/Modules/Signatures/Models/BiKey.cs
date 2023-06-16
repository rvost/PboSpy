using PboSpy.Modules.Signatures.Utils;
using System.IO;
using System.Text;

namespace PboSpy.Modules.Signatures.Models;

// TODO: Use required modifier after update to C# 11 
public record BiKey
{
    public string Name { get; init; }

    public UInt32 Length { get; init; }

    public UInt32 Exponent { get; init; }

    public byte[] N { get; init; }

    public static BiKey ReadFromStream(Stream input, bool leaveOpen = false)
    {
        using var reader = new BinaryReader(input, Encoding.UTF8, leaveOpen);

        var (_, name) = reader.ReadCString();

        var temp = reader.ReadUInt32();

        // unknown
        reader.ReadUInt32();
        reader.ReadUInt32();
        reader.ReadUInt32();

        var length = reader.ReadUInt32();
        var exponent = reader.ReadUInt32();

        if (temp != length / 8 + 20)
        {
            throw new InvalidOperationException();
        }

        var n = reader.ReadBytes((int)length / 8);

        return new()
        {
            Name = name,
            Length = length,
            Exponent = exponent,
            N = n
        };
    }

    public void WriteToStream(Stream output, bool leaveOpen = false)
    {
        using var writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen);

        writer.WriteCString(Name);
        writer.Write(Length / 8 + 20);

        // TODO: Use UTF-8 string literals after update to C# 11 
        var unknown = new byte[] { 0x06, 0x02, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00 };
        var rsa = new byte[] { 0x52, 0x53, 0x41, 0x31 }; // "RSA1"
        writer.Write(unknown);
        writer.Write(rsa);

        writer.Write(Length);
        writer.Write(Exponent);
        writer.Write(N);
    }

    public static BiKey FromSignature(BiSign signature)
    {
        return new()
        {
            Name = signature.Name,
            Length = signature.Length,
            Exponent = signature.Exponent,
            N = signature.N.ToArray()
        };
    }
}
