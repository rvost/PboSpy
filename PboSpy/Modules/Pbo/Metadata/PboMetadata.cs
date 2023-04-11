using PboSpy.Modules.Metadata;
using PboSpy.Modules.Metadata.Utils;
using PboSpy.Modules.Pbo.Models;
using System.ComponentModel;
using System.IO;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace PboSpy.Modules.Pbo.Metadata;

[DisplayName("PBO")]
class PboMetadata : IMetadata
{
    [Browsable(false)]
    public string Name => System.IO.Path.GetFileName(Path);

    [Description("Number of files in PBO")]
    public string Path { get; set; }

    [Description("Size of PBO on disk")]
    public string Size { get; set; }

    [Description("Number of files in PBO")]
    public int Entries { get; set; }

    [Description("PBO Prefix")]
    public string Prefix { get; set; }

    [Editor(typeof(CollectionEditor), typeof(CollectionEditor))]
    public ICollection<KeyValuePair<string, string>> Properties { get; }

    public PboMetadata(PboFile file)
    {
        Path = file.PBO.PBOFilePath;
        Size = Formatters.FormatSize(new FileInfo(file.PBO.PBOFilePath).Length);
        Entries = file.PBO.Files.Count;
        Prefix = file.PBO.Prefix;
        Properties = file.PBO.PropertiesPairs;
    }
}
