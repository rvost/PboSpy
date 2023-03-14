using System.IO;
using System.Text;
using PboSpy.Modules.Signatures.Utils;

namespace PboSpy.Modules.Signatures.Models;

// TODO: Use required modifier after update to C# 11 
public record BiSign
{
    public BiSignVersion Version { get; init; }
    public string Name { get; init; }

    public UInt32 Length { get; init; }

    public UInt32 Exponent { get; init; }

    public byte[] N { get; init; }

    public byte[] Sig1 { get; init; }

    public byte[] Sig2 { get; init; }

    public byte[] Sig3 { get; init; }

    public static BiSign ReadFromStream(Stream input, bool leaveOpen = false)
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

        reader.ReadUInt32();
        var sig1 = reader.ReadBytes((int)length / 8);

        var version = (BiSignVersion)reader.ReadUInt32();

        reader.ReadUInt32();
        var sig2 = reader.ReadBytes((int)length / 8);

        reader.ReadUInt32();
        var sig3 = reader.ReadBytes((int)length / 8);

        return new() { 
            Version = version,
            Name = name,
            Length = length,
            Exponent = exponent,
            N=n,
            Sig1=sig1,
            Sig2=sig2,
            Sig3=sig3
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
        writer.Write(Length/8);
        writer.Write(Sig1);
        writer.Write((UInt32)Version);
        writer.Write(Length / 8);
        writer.Write(Sig2);
        writer.Write(Length / 8);
        writer.Write(Sig3);
    }
}