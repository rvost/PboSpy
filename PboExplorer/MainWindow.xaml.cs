using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BIS.Core.Streams;
using BIS.PBO;
using BIS.WRP;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PboExplorer.Helpers;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Models;

namespace PboExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<ITreeItem> PboList = new ObservableCollection<ITreeItem>();
        private PboFile CurrentPBO { get; set; }
        private PboEntry SelectedEntry { get; set; }
        internal ICollection<ConfigClassItem> MergedConfig { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            PboView.ItemsSource = PboList;

            var list = new List<string>();
            foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
            {
                if (File.Exists(arg))
                {
                    list.Add(arg);
                }
                else if (Directory.Exists(arg))
                {
                    list.AddRange(DirectoryExtensions.GetSupportedFiles(arg));
                }
            }
            if (list.Count > 0)
            {
                LoadSupportedFiles(list);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Load PBO archive";
            dlg.DefaultExt = ".pbo";
            dlg.Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == true)
            {
                LoadSupportedFiles(dlg.FileNames);
            }
        }
        private void OpenDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Load PBO archives from a directory";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LoadSupportedFiles(DirectoryExtensions.GetSupportedFiles(dialog.FileName));
            }
        }

        private void CloseAll(object sender, RoutedEventArgs e)
        {
            ResetView();
            AboutBox.Visibility = Visibility.Visible;
            PboList.Clear();
            MergedConfig = null;
            DataView.ItemsSource = null;
            ConfigView.ItemsSource = null;
        }

        private void About(object sender, RoutedEventArgs e)
        {
            ResetView();
            AboutBox.Visibility = Visibility.Visible;
        }

        private void LoadSupportedFiles(IEnumerable<string> fileNames)
        {
            var lookup = fileNames.ToLookup(f => string.Equals(Path.GetExtension(f), ".pbo", StringComparison.OrdinalIgnoreCase));
            var pbos = lookup[true];
            var nonPbos = lookup[false];

            Task.Factory
                .StartNew(() => pbos.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                .Select(fileName => new PboFile(new PBO(fileName, false))))
                .ContinueWith((r) =>
                {
                    foreach (var e in r.Result)
                    {
                        PboList.Add(e);
                    }
                    GenerateMergedConfig(PboList.OfType<PboFile>());
                }, TaskScheduler.FromCurrentSynchronizationContext());

            var filesToAdd = nonPbos
                .Where(file => File.Exists(file))
                .Select(file => new PhysicalFile(Path.GetFullPath(file)))
                .ToList();

            if (filesToAdd.Any())
            {
                var openedFiles = PboList.OfType<PhysicalFiles>().FirstOrDefault();

                if (openedFiles is null)
                {
                    openedFiles = new PhysicalFiles();
                    PboList.Add(openedFiles);
                }

                foreach (var file in filesToAdd)
                {
                    openedFiles.AddEntry(file);
                }
            }
        }

        private void GenerateMergedConfig(IEnumerable<PboFile> files)
        {
            DataView.ItemsSource = PboFile.MergedView(files).Children;
            MergedConfig = ConfigClassItem.MergedView(files);
            ConfigView.ItemsSource = MergedConfig;
        }

        private void ExtractCurrentPBO(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Extract to";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                CurrentPBO.Extract(dialog.FileName);
            }
        }
        private void CanExtractSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedEntry != null;
        }

        private void ExtractSelected(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Title = "Extract";
                dlg.FileName = SelectedEntry.Name;
                dlg.Filter = "All files|*.*";
                if (dlg.ShowDialog() == true)
                {
                    SelectedEntry.Extract(dlg.FileName);
                }
            }
        }
        private void ReplaceSelected(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "Replace";
                dlg.FileName = SelectedEntry.Name;
                dlg.Filter = SelectedEntry.Name + "|" + SelectedEntry.Name + "|*" + SelectedEntry.Extension + "|*" + SelectedEntry.Extension;
                if (dlg.ShowDialog() == true)
                {
                    var pbo = SelectedEntry.PBO;
                    var index = pbo.Files.IndexOf(SelectedEntry.Entry);
                    pbo.Files[index] = new PBOFileToAdd(new FileInfo(dlg.FileName), SelectedEntry.Entry.FileName);
                    pbo.Save();
                    RefreshEntries(pbo);
                }
            }
        }

        private void RefreshEntries(PBO pbo)
        {
            ResetView();
            var view = PboList.OfType<PboFile>().FirstOrDefault(p => p.PBO == pbo);
            if (view != null)
            {
                view.RefreshEntries();
            }
        }

        private void ExtractSelectedAsText(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Title = "Extract to text file";
                dlg.FileName = SelectedEntry.Extension == ".bin" ? Path.ChangeExtension(SelectedEntry.Name, ".cpp") : SelectedEntry.Name;
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text file|*.txt|CPP|*.cpp|HPP|*.hpp|SQM|*.sqm|SQF|*.sqf|RVMAT|*.rvmat";
                if (dlg.ShowDialog() == true)
                {
                    File.WriteAllText(dlg.FileName, TextPreview.Text);
                }
            }
        }

        private void ExtractSelectedAsPNG(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Title = "Extract to PNG";
                dlg.FileName = Path.ChangeExtension(SelectedEntry.Name, ".png");
                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG|*.png";
                if (dlg.ShowDialog() == true)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ImagePreview.Source));
                    using (var stream = File.Create(dlg.FileName))
                    {
                        encoder.Save(stream);
                    }
                }
            }
        }

        private void OnConfigViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetView();
            Cursor = Cursors.Wait;
            var entry = e.NewValue as ConfigClassItem;
            if (entry != null)
            {
                PropertiesGrid.ItemsSource = entry.GetAllProperties().Select(p => new PropertyItem(p.Key, p.Value?.ToString() ?? "(null)")).ToList();
                var sb = new StringBuilder();
                foreach (var def in entry.Definitions)
                {
                    sb.AppendFormat("// Defined by '{1}' (in '{0}')", def.Item1.PBO.PBOFilePath, def.Item1.FullPath);
                    sb.AppendLine();
                    sb.Append(def.Item2.ToString(0));
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                }
                ShowText(sb.ToString());

            }
            Cursor = Cursors.Arrow;
        }

        private void OnPboViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetView();
            Cursor = Cursors.Wait;
            if (e.NewValue is PboEntry entry)
            {
                SelectedEntry = entry;
                Show(entry);
            }
            else if (e.NewValue is PboFile file)
            {
                CurrentPBO = file;
                Show(file);
            }
            else if (e.NewValue is PboDirectory directory)
            {
                Show(directory);
            }
            else if (e.NewValue is PhysicalFile pfile)
            {
                Show(pfile);
            }
            Cursor = Cursors.Arrow;
        }

        private void ResetView()
        {
            AboutBox.Visibility = Visibility.Hidden;
            TextPreview.Visibility = Visibility.Hidden;
            ImagePreview.Reset();
            ImagePreview.Visibility = Visibility.Hidden;
            ImagePreview.Source = null;
            TextPreview.Text = string.Empty;

            ExtractFilePNG.IsEnabled = false;
            ExtractFileText.IsEnabled = false;
            ExtractPBO.IsEnabled = false;
            SelectedEntry = null;
            CurrentPBO = null;
            PropertiesGrid.ItemsSource = null;
        }

        private void Show(PboFile file)
        {
            ExtractPBO.IsEnabled = true;
            var infos = new List<PropertyItem>()
            {
                new PropertyItem("PBO File", file.PBO.PBOFilePath),
                new PropertyItem("Size", Formatters.FormatSize(new FileInfo(file.PBO.PBOFilePath).Length)),
                new PropertyItem("Entries", file.PBO.Files.Count.ToString()),
                new PropertyItem("Prefix", file.PBO.Prefix),
            };
            foreach (var pair in file.PBO.PropertiesPairs)
            {
                infos.Add(new PropertyItem($"Property '{pair.Key}'", pair.Value));
            }
            PropertiesGrid.ItemsSource = infos;
        }
        private void Show(PhysicalFile entry)
        {

            var infos = new List<PropertyItem>()
            {
                new PropertyItem("Full path", entry.FullPath)
            };

            Show(entry, infos);

        }

        private void Show(PboEntry entry)
        {
            var infos = new List<PropertyItem>()
            {
                new PropertyItem("PBO File", entry.PBO.PBOFilePath),
                new PropertyItem("Entry name", entry.Entry.FileName),
                new PropertyItem("Entry full path", entry.FullPath),
                new PropertyItem("TimeStamp", PBO.Epoch.AddSeconds(entry.Entry.TimeStamp).ToString()),
            };

            if (entry.Entry.IsCompressed)
            {
                infos.Add(new PropertyItem("Size uncompressed", Formatters.FormatSize(entry.Entry.Size)));
                infos.Add(new PropertyItem("Size in PBO", Formatters.FormatSize(entry.Entry.DiskSize)));
            }
            else
            {
                infos.Add(new PropertyItem("Size", Formatters.FormatSize(entry.Entry.Size)));
            }

            Show(entry, infos);
        }

        private void Show(FileBase entry, List<PropertyItem> infos)
        {
            try
            {
                switch (entry.Extension)
                {
                    case ".paa":
                    case ".pac":
                        ShowPAA(entry, infos);
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        ShowImage(entry, infos);
                        break;
                    case ".rvmat":
                    case ".sqm":
                        ShowDetectConfig(entry, infos);
                        break;
                    case ".wrp":
                        ShowWRP(entry, infos);
                        break;
                    case ".p3d":
                        ShowP3D(entry, infos);
                        break;
                    case ".rtm":
                    case ".wss":
                    case ".ogg":
                    case ".bin":
                    case ".fxy":
                    case ".wsi":
                    case ".shp":
                    case ".dbf":
                    case ".shx":
                    case ".bisurf":
                        ShowGenericBinary(entry, infos);
                        break;
                    default:
                        ShowGenericText(entry, infos);
                        break;

                }
            }
            catch (Exception e)
            {
                ShowText(e.ToString());
            }
            PropertiesGrid.ItemsSource = infos;
        }

        private void ShowP3D(FileBase entry, List<PropertyItem> infos)
        {
            using (var stream = entry.GetStream())
            {
                var p3d = StreamHelper.Read<BIS.P3D.P3D>(stream);
                infos.Add(new PropertyItem("Type", p3d.IsEditable ? "MLOD" : "ODOL"));
                infos.Add(new PropertyItem("Bbox Max", p3d.ModelInfo.BboxMax.ToString()));
                infos.Add(new PropertyItem("Bbox Min", p3d.ModelInfo.BboxMin.ToString()));
                infos.Add(new PropertyItem("MapType", p3d.ModelInfo.MapType.ToString()));
                infos.Add(new PropertyItem("Class", p3d.ModelInfo.Class.ToString()));
                infos.Add(new PropertyItem("Version", p3d.Version.ToString()));
                infos.Add(new PropertyItem("LODs", p3d.LODs.Count().ToString()));

                var sb = new StringBuilder();
                foreach (var lod in p3d.LODs)
                {
                    sb.AppendLine("---------------------------------------------------------------------------------------------------");
                    sb.AppendLine($"LOD {lod.Resolution}");
                    sb.AppendLine($"    {lod.FaceCount} Faces, {lod.VertexCount} Vertexes, {lod.GetModelHashId()}");
                    sb.AppendLine($"    Named properties");
                    foreach (var prop in lod.NamedProperties.OrderBy(p => p.Item1))
                    {
                        sb.AppendLine($"        {prop.Item1} = {prop.Item2}");
                    }
                    sb.AppendLine($"    Named selections");
                    foreach (var prop in lod.NamedSelections.OrderBy(m => m.Name))
                    {
                        var mat = prop.Material;
                        var tex = prop.Texture;
                        if (!string.IsNullOrEmpty(mat) || !string.IsNullOrEmpty(tex))
                        {
                            sb.AppendLine($"        {prop.Name} (material='{mat}' texture='{tex}')");
                        }
                        else
                        {
                            sb.AppendLine($"        {prop.Name}");
                        }
                    }
                    sb.AppendLine($"    Textures");
                    foreach (var prop in lod.GetTextures().OrderBy(m => m))
                    {
                        sb.AppendLine($"        {prop}");
                    }
                    sb.AppendLine($"    Materials");
                    foreach (var prop in lod.GetMaterials().OrderBy(m => m))
                    {
                        sb.AppendLine($"        {prop}");
                    }
                    sb.AppendLine();
                }
                TextPreview.Text = sb.ToString();
                TextPreview.Visibility = Visibility.Visible;
            }
        }

        private void ShowWRP(FileBase entry, List<PropertyItem> infos)
        {
            using (var stream = entry.GetStream())
            {
                var wrp = StreamHelper.Read<AnyWrp>(stream);
                infos.Add(new PropertyItem("CellSize", wrp.CellSize.ToString()));
                infos.Add(new PropertyItem("LandRange", $"{wrp.LandRangeX}x{wrp.LandRangeY}"));
                infos.Add(new PropertyItem("TerrainRange", $"{wrp.TerrainRangeX}x{wrp.TerrainRangeY}"));
                infos.Add(new PropertyItem("Objects.Count", wrp.ObjectsCount.ToString()));
                infos.Add(new PropertyItem("Materials.Count", wrp.MatNames.Length.ToString()));
                ImagePreview.Source = wrp.PreviewElevation();
                ImagePreview.Visibility = Visibility.Visible;
            }
        }

        private void ShowPAA(FileBase entry, List<PropertyItem> infos)
        {
            ExtractFilePNG.IsEnabled = true;
            var paa = entry.GetPaaImage();
            infos.Add(new PropertyItem("Image size", $"{paa.Paa.Width}x{paa.Paa.Height}"));
            infos.Add(new PropertyItem("Image type", paa.Paa.Type.ToString()));
            ImagePreview.Source = paa.Bitmap;
            ImagePreview.Visibility = Visibility.Visible;
        }

        private void ShowGenericText(FileBase entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetText());
        }

        private void ShowText(string text)
        {
            ExtractFileText.IsEnabled = true;
            TextPreview.Text = text;
            TextPreview.Visibility = Visibility.Visible;
        }

        private void ShowDetectConfig(FileBase entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetDetectConfigAsText(out bool wasBinary));
            infos.Add(new PropertyItem("Format", wasBinary ? "Binarized" : "Text"));
        }

        private void ShowGenericBinary(FileBase entry, List<PropertyItem> infos)
        {
            if (entry.IsBinaryConfig())
            {
                ShowBinaryConfig(entry, infos);
                return;
            }
        }

        private void ShowBinaryConfig(FileBase entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetBinaryConfigAsText());
        }

        private void ShowImage(FileBase entry, List<PropertyItem> infos)
        {
            ExtractFilePNG.IsEnabled = true;
            using (var stream = entry.GetStream())
            {
                ImagePreview.Source = BitmapFrame.Create(stream,
                                                  BitmapCreateOptions.None,
                                                  BitmapCacheOption.OnLoad);
            }
            ImagePreview.Visibility = Visibility.Visible;
        }

        private void Show(PboDirectory directory)
        {
            PropertiesGrid.ItemsSource = new List<PropertyItem>()
            {
                new PropertyItem("Size uncompressed", Formatters.FormatSize(directory.UncompressedSize)),
                new PropertyItem("Size in PBO", Formatters.FormatSize(directory.DataSize)),
            };
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextPreview.Text))
            {
                Clipboard.SetText(TextPreview.Text);
            }
            else if (ImagePreview.Source is BitmapSource bmp)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (var pngMemStream = new MemoryStream())
                {
                    encoder.Save(pngMemStream);
                    var data = new DataObject();
                    data.SetImage(bmp); // For applications that does not support PNG data
                    data.SetData("PNG", pngMemStream, false);
                    Clipboard.SetDataObject(data, true);
                }
            }
        }

        private void CanCopyToClipboard(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(TextPreview.Text) || ImagePreview.Source is BitmapSource;
        }

        private void PboFiles_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Split folders and files
                var lookup = paths.ToLookup(
                    (path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                    );

                // Load files from folders
                lookup[true].ToList().ForEach(
                    dir => LoadSupportedFiles(DirectoryExtensions.GetSupportedFiles(dir))
                    );

                // Load other files
                LoadSupportedFiles(lookup[false]);
            }
        }
    }
}
