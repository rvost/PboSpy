using System.IO;

namespace PboSpy.Models;

public abstract class FileBase
{
    public abstract Stream GetStream();

    public abstract string Extension { get; }
    public abstract string Name { get; }
    public abstract string FullPath { get; }
    public abstract int DataSize { get; }

    public override bool Equals(object obj)
    {
        return obj is FileBase @base &&
               FullPath == @base.FullPath;
    }
}
