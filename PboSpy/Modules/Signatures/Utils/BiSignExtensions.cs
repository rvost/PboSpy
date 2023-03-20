using PboSpy.Modules.Signatures.Models;
using System.Windows.Media;
using WpfHexaEditor.Core;

namespace PboSpy.Modules.Signatures.Utils;

internal static class BiSignExtensions
{
    public static List<CustomBackgroundBlock> GetHighlighting(this BiSign signature)
    {
        var sigLength = signature.Length / 8;

        var lengthOffset = signature.Name.Length + 1 + 4 + 12;
        var exponentOffset = lengthOffset + 4;
        var nOffset = exponentOffset + 4;
        var sig1Offset = nOffset + sigLength + 4;
        var versionOffset = sig1Offset + sigLength;
        var sig2Offset = versionOffset + 4 + 4;
        var sig3Offset = sig2Offset + sigLength + 4;

        return new List<CustomBackgroundBlock>()
        {
            new CustomBackgroundBlock(0, signature.Name.Length, Brushes.Gold, "Signature name"),
            new CustomBackgroundBlock(lengthOffset, 4, Brushes.Coral, "Key length"),
            new CustomBackgroundBlock(exponentOffset, 4, Brushes.LightGoldenrodYellow, "Key exponent"),
            new CustomBackgroundBlock(exponentOffset, sigLength, Brushes.Tomato, "Key N"),
            new CustomBackgroundBlock(sig1Offset, sigLength, Brushes.SkyBlue, "Signature 1"),
            new CustomBackgroundBlock(versionOffset, 4, Brushes.Lime, "Signature verion"),
            new CustomBackgroundBlock(sig2Offset, sigLength, Brushes.LightSalmon, "Signature 2"),
            new CustomBackgroundBlock(sig3Offset, sigLength, Brushes.PaleGreen, "Signature 3"),
        };
    }
}
