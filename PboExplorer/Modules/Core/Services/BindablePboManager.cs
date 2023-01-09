using BIS.PBO;
using Caliburn.Micro;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Services;

[Export(typeof(IPboManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class BindablePboManager : IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }
    public ICollection<ITreeItem> ConfigTree { get; }

    public BindablePboManager()
    {
        FileTree = new BindableCollection<ITreeItem>();
        ConfigTree = new BindableCollection<ITreeItem>();
    }

    public void LoadSupportedFiles(IEnumerable<string> paths)
    {
        // Split folders and files
        var lookup = paths.ToLookup(
            (path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory)
            );

        // Load files from folders
        lookup[true].ToList().ForEach(
            dir => LoadFiles(GetSupportedFiles(dir))
            );

        // Load other files
        LoadFiles(lookup[false]);
    }

    private void LoadFiles(IEnumerable<string> fileNames)
    {
        var lookup = fileNames.ToLookup(f => string.Equals(Path.GetExtension(f), ".pbo", StringComparison.OrdinalIgnoreCase));
        var pbos = lookup[true];
        var nonPbos = lookup[false];

        LoadPbos(pbos);

        LoadPhysicalFiles(nonPbos);
    }

    private void LoadPbos(IEnumerable<string> paths)
    {
        Task.Factory
            .StartNew(() => paths.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .Select(fileName => new PboFile(new PBO(fileName, false))))
            .ContinueWith((r) =>
            {
                foreach (var e in r.Result)
                {
                    FileTree.Add(e);
                }
                GenerateMergedConfig(FileTree.OfType<PboFile>());
            });
    }

    private void LoadPhysicalFiles(IEnumerable<string> paths)
    {
        var filesToAdd = paths
            .Where(file => File.Exists(file))
            .Select(file => new PhysicalFile(Path.GetFullPath(file)))
            .ToList();

        if (filesToAdd.Any())
        {
            var openedFiles = FileTree.OfType<PhysicalFiles>().FirstOrDefault();

            if (openedFiles is null)
            {
                openedFiles = new PhysicalFiles();
                FileTree.Add(openedFiles);
            }

            foreach (var file in filesToAdd)
            {
                openedFiles.AddEntry(file);
            }
        }
    }

    private void GenerateMergedConfig(IEnumerable<PboFile> files)
    {
        var configClasses = ConfigClassItem.MergedView(files);
        foreach (var configClass in configClasses)
        {
            ConfigTree.Add(configClass);
        }
    }

    private static IEnumerable<string> GetSupportedFiles(string arg)
    {
        HashSet<string> _supportedExtensions = new() { ".pbo", ".paa", ".rvmat", ".bin",
        ".pac", "*.p3d", "*.wrp", "*.sqm" };

        return Directory.EnumerateFiles(arg, "*.*", SearchOption.AllDirectories)
            .Where(path => _supportedExtensions.Contains(Path.GetExtension(path)));
    }
}