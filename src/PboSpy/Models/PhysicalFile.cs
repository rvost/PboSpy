﻿using PboSpy.Interfaces;
using System.IO;

namespace PboSpy.Models;

public class PhysicalFile : FileBase, ITreeItem, IPersistentItem
{
    public PhysicalFile(string fullPath, ITreeItem parent = null)
    {
        FullPath = fullPath;
        Parent = parent;
    }

    public ICollection<ITreeItem> Children => null;

    public override string Extension => System.IO.Path.GetExtension(FullPath);

    public override string Name => System.IO.Path.GetFileName(FullPath);

    public string Path => FullPath;

    string ITreeItem.Name => FullPath;

    public override string FullPath { get; }
    public ITreeItem Parent { get; set; }

    public override int DataSize => (int)new FileInfo(FullPath).Length;

    public override Stream GetStream()
    {
        return File.OpenRead(FullPath);
    }
}
