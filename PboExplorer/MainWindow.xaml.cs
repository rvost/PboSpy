using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BIS.Core.Streams;
using BIS.PBO;
using BIS.WRP;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using SixLabors.ImageSharp.PixelFormats;

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

        private PhysicalFiles physicalFiles;
        public MainWindow()
        {
            InitializeComponent();
            PboView.ItemsSource = PboList;

            var list = new List<string>();
            foreach(var arg in Environment.GetCommandLineArgs().Skip(1))
            {
                if (File.Exists(arg))
                {
                    list.Add(arg);
                }
                else if (Directory.Exists(arg))
                {
                    list.AddRange(Directory.GetFiles(arg, "*.pbo", SearchOption.AllDirectories));
                }
            }
            if (list.Count > 0)
            {
                LoadPboList(list);
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
                LoadPboList(dlg.FileNames);
            }
        }
        private void OpenDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Load PBO archives from a directory";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LoadPboList(Directory.GetFiles(dialog.FileName, "*.pbo", SearchOption.AllDirectories));
            }
        }

        private void CloseAll(object sender, RoutedEventArgs e)
        {
            ResetView();
            AboutBox.Visibility = Visibility.Visible;
            PboList.Clear();
            DataView.ItemsSource = null;
            ConfigView.ItemsSource = null;
        }

        private void About(object sender, RoutedEventArgs e)
        {
            ResetView();
            AboutBox.Visibility = Visibility.Visible;
        }

        private void LoadPboList(IEnumerable<string> fileNames)
        {
            var pbos = fileNames.Where(f => string.Equals(System.IO.Path.GetExtension(f), ".pbo", StringComparison.OrdinalIgnoreCase)).ToList();
            var nonPbos = fileNames.Where(f => !string.Equals(System.IO.Path.GetExtension(f), ".pbo", StringComparison.OrdinalIgnoreCase)).ToList();

            Task.Factory
                .StartNew(() => pbos.OrderBy(f => System.IO.Path.GetFileName(f), StringComparer.OrdinalIgnoreCase).Select(fileName => new PboFile(new PBO(fileName, false))))
                .ContinueWith((r) =>
                {
                    foreach(var e in r.Result)
                    {
                        PboList.Add(e);
                    }
                    GenerateMerged(PboList.OfType<PboFile>());
                }, TaskScheduler.FromCurrentSynchronizationContext());

            PhysicalFile lastFile = null;
            foreach (var file in nonPbos)
            {
                if (File.Exists(file))
                {
                    lastFile = OpenNonPbo(System.IO.Path.GetFullPath(file));
                }
            }
            if (lastFile != null)
            {
                var files = (TreeViewItem)PboView.ItemContainerGenerator.ContainerFromItem(physicalFiles);
                files.IsExpanded = true;

                if (files.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    files.ItemContainerGenerator.StatusChanged += (_, _) =>
                    {
                        var file = (TreeViewItem)files.ItemContainerGenerator.ContainerFromItem(lastFile);
                        if (file != null)
                        {
                            file.IsSelected = true;
                        }
                    };
                }
                else
                {
                    var file = (TreeViewItem)files.ItemContainerGenerator.ContainerFromItem(lastFile);
                    if (file != null)
                    {
                        file.IsSelected = true;
                    }
                }
            }
        }

        private void GenerateMerged(IEnumerable<PboFile> files)
        {
            DataView.ItemsSource = PboFile.MergedView(files).Children;
            ConfigView.ItemsSource = ConfigClassItem.MergedView(files);
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

        private void ExtractSelectedAsText(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Title = "Extract to text file";
                dlg.FileName = SelectedEntry.Extension == ".bin" ? System.IO.Path.ChangeExtension(SelectedEntry.Name, ".cpp") : SelectedEntry.Name;
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
                dlg.FileName = System.IO.Path.ChangeExtension(SelectedEntry.Name, ".png");
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

        private void ShowConfigClassEntry(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetView();
            Cursor = Cursors.Wait;
            var entry = e.NewValue as ConfigClassItem;
            if ( entry != null)
            {
                PropertiesGrid.ItemsSource = entry.GetAllProperties().Select(p => new PropertyItem(p.Key, p.Value?.ToString() ?? "(null)")).ToList();
                var sb = new StringBuilder();
                foreach(var def in entry.Definitions)
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

        private void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e)
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
            ImagePreviewBorder.Reset();
            ImagePreviewBorder.Visibility = Visibility.Hidden;
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
            var infos =  new List<PropertyItem>()
            {
                new PropertyItem("PBO File", file.PBO.PBOFilePath),
                new PropertyItem("Size", FormatSize(new FileInfo(file.PBO.PBOFilePath).Length)),
                new PropertyItem("Entries", file.PBO.FileEntries.Count.ToString()),
                new PropertyItem("Prefix", file.PBO.Prefix),
            };
            var props = file.PBO.Properties.ToArray();
            for (int i = 0; i < props.Length; i += 2 )
            {
                if ( i + 1 == props.Length)
                {
                    infos.Add(new PropertyItem(props[i], ""));
                }
                else
                {
                    infos.Add(new PropertyItem($"Property '{props[i]}'", props[i + 1]));
                }
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
                new PropertyItem("TimeStamp", entry.Entry.TimeStamp.ToString()),
            };

            if (entry.Entry.IsCompressed)
            {
                infos.Add(new PropertyItem("Size uncompressed", FormatSize(entry.Entry.UncompressedSize)));
                infos.Add(new PropertyItem("Size in PBO", FormatSize(entry.Entry.DataSize)));
            }
            else
            {
                infos.Add(new PropertyItem("Size", FormatSize(entry.Entry.DataSize)));
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
                    case ".rtm":
                    case ".wss":
                    case ".ogg":
                    case ".bin":
                    case ".fxy":
                    case ".wsi":
                    case ".shp":
                    case ".dbf":
                    case ".shx":
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

        private void ShowWRP(FileBase entry, List<PropertyItem> infos)
        {
            using(var stream = entry.GetStream())
            {
                var wrp = StreamHelper.Read<AnyWrp>(stream);
                infos.Add(new PropertyItem("CellSize", wrp.CellSize.ToString()));
                infos.Add(new PropertyItem("LandRange", $"{wrp.LandRangeX}x{wrp.LandRangeY}"));
                infos.Add(new PropertyItem("TerrainRange", $"{wrp.TerrainRangeX}x{wrp.TerrainRangeY}"));
                infos.Add(new PropertyItem("Objects.Count", wrp.ObjectsCount.ToString()));
                infos.Add(new PropertyItem("Materials.Count", wrp.MatNames.Length.ToString()));
                ImagePreview.Source = PreviewElevation(wrp);
                ImagePreviewBorder.Visibility = Visibility.Visible;
            }


        }
        public BitmapSource PreviewElevation(AnyWrp wrp)
        {
            var min = 4000d;

            var max = -1000d;

            for (int y = 0; y < wrp.TerrainRangeY; y++)
            {
                for (int x = 0; x < wrp.TerrainRangeX; x++)
                {
                    max = Math.Max(wrp.Elevation[x + (y * wrp.TerrainRangeY)], max);
                    min = Math.Min(wrp.Elevation[x + (y * wrp.TerrainRangeY)], min);
                }
            }

            var min0 = Math.Min(-1, min);
            min = Math.Max(0, min);
            var legend = new[]
            {
                new { E = min0, Color = SixLabors.ImageSharp.Color.DarkBlue.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min, Color = SixLabors.ImageSharp.Color.LightBlue.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.10, Color = SixLabors.ImageSharp.Color.DarkGreen.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.15, Color = SixLabors.ImageSharp.Color.Green.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.40, Color = SixLabors.ImageSharp.Color.Yellow.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = min + (max - min) * 0.70, Color = SixLabors.ImageSharp.Color.Red.ToPixel<Rgb24>().ToScaledVector4() },
                new { E = max, Color = SixLabors.ImageSharp.Color.Maroon.ToPixel<Rgb24>().ToScaledVector4() }
            };
            var img = new SixLabors.ImageSharp.Image<Bgra32>(wrp.TerrainRangeX, wrp.TerrainRangeY);
            for (int y = 0; y < wrp.TerrainRangeY; y++)
            {
                for (int x = 0; x < wrp.TerrainRangeX; x++)
                {
                    var elevation = wrp.Elevation[x + (y * wrp.TerrainRangeY)];
                    var before = legend.Where(e => e.E <= elevation).Last();
                    var after = legend.FirstOrDefault(e => e.E > elevation) ?? legend.Last();
                    var scale = (float)((elevation - before.E) / (after.E - before.E));
                    Bgra32 rgb = new Bgra32();
                    rgb.FromScaledVector4(Vector4.Lerp(before.Color, after.Color, scale));
                    img[x, wrp.TerrainRangeY - y - 1] = rgb;
                }
            }
            return new ImageSharpImageSource(img);
        }

        private void ShowPAA(FileBase entry, List<PropertyItem> infos)
        {
            ExtractFilePNG.IsEnabled = true;
            var paa = entry.GetPaaImage();
            infos.Add(new PropertyItem("Image size", $"{paa.Paa.Width}x{paa.Paa.Height}"));
            infos.Add(new PropertyItem("Image type", paa.Paa.Type.ToString()));
            ImagePreview.Source = paa.Bitmap;
            ImagePreviewBorder.Visibility = Visibility.Visible;
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
            ImagePreviewBorder.Visibility = Visibility.Visible;
        }

        private void Show(PboDirectory directory)
        {
            PropertiesGrid.ItemsSource = new List<PropertyItem>()
            {
                new PropertyItem("Size uncompressed", FormatSize(directory.UncompressedSize)),
                new PropertyItem("Size in PBO", FormatSize(directory.DataSize)),
            };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private static string FormatSize(double size)
        {
            string[] sizes = { "Bytes", "KiB", "MiB", "GiB", "TiB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }
            return string.Format("{0:0.##} {1}", size, sizes[order]);
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

        internal PhysicalFile OpenNonPbo(string fullPath)
        {
            if (physicalFiles == null)
            { 
                physicalFiles = new PhysicalFiles();
                PboList.Add(physicalFiles);
            }
            var file = new PhysicalFile(fullPath);
            physicalFiles.AddEntry(file);
            return file;
        }
    }
}
