using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;

namespace PboExplorer.Modules.Core.Models;

class PhysicalFile : FileBase, ITreeItem
{
    private readonly Lazy<PhysicalFileMetadata> metadata;
    public PhysicalFile(string fullPath)
    {
        FullPath = fullPath;
        metadata = new Lazy<PhysicalFileMetadata>(() => new(this));
    }

    public ICollection<ITreeItem> Children => null;

    public override string Extension => Path.GetExtension(FullPath);

    public override string Name => Path.GetFileName(FullPath);

    string ITreeItem.Name => FullPath;

    public override string FullPath { get; }

    public override int DataSize => (int)new FileInfo(FullPath).Length;

    public IMetadata Metadata => metadata.Value;

    public override Stream GetStream()
    {
        return File.OpenRead(FullPath);
    }
}

class PhysicalFileMetadata : IMetadata
{
    [DisplayName("Full Path")]
    public string FullPath { get; set; }
    [DisplayName("File Name")]
    public string Name { get; set; }
    public string Size { get; set; }
    [DisplayName("Created At")]
    public DateTime CreatedAt { get; set; }

    public PhysicalFileMetadata(PhysicalFile file)
    {
        FullPath = file.FullPath;
        var info = new FileInfo(file.FullPath);
        Name = info.Name;
        Size = Formatters.FormatSize(info.Length);
        CreatedAt = info.CreationTime;
    }
}