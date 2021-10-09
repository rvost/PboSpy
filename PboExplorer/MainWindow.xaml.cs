using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BIS.PAA;
using BIS.PBO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PboExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<PboFile> PboList = new ObservableCollection<PboFile>();
        private PboFile CurrentPBO { get; set; }
        private PboEntry SelectedEntry { get; set; }

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
            dlg.Filter = "PBO File|*.pbo";
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
        }

        private void About(object sender, RoutedEventArgs e)
        {
            ResetView();
            AboutBox.Visibility = Visibility.Visible;
        }

        private void LoadPboList(IEnumerable<string> fileNames)
        {
            Task.Factory
                .StartNew(() => fileNames.OrderBy(f => System.IO.Path.GetFileName(f), StringComparer.OrdinalIgnoreCase).Select(fileName => new PboFile(new PBO(fileName, false))))
                .ContinueWith((r) =>
                {
                    foreach(var e in r.Result)
                    {
                        PboList.Add(e);
                    }
                    GenerateMerged(r.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void GenerateMerged(IEnumerable<PboFile> files)
        {
            DataView.ItemsSource = PboFile.MergedView(files).Children;
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

        private void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetView();

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
        }

        private void ResetView()
        {
            AboutBox.Visibility = Visibility.Hidden;
            TextPreview.Visibility = Visibility.Hidden;
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
                    case ".p3d":
                    case ".rtm":
                    case ".wss":
                    case ".wrp":
                    case ".ogg":
                    case ".bin":
                    case ".fxy":
                    case ".wsi":
                        ShowGenericBinary(entry, infos);
                        break;
                    default:
                        ShowGenericText(entry, infos);
                        break;

                }
            }
            catch(Exception e)
            {
                ShowText(e.ToString());
            }
            PropertiesGrid.ItemsSource = infos;
        }

        private void ShowPAA(PboEntry entry, List<PropertyItem> infos)
        {
            ExtractFilePNG.IsEnabled = true;
            var paa = entry.GetPaaImage();
            infos.Add(new PropertyItem("Image size", $"{paa.Paa.Width}x{paa.Paa.Height}"));
            infos.Add(new PropertyItem("Image type", paa.Paa.Type.ToString()));
            ImagePreview.Source = paa.Bitmap;
            ImagePreview.Visibility = Visibility.Visible;
        }

        private void ShowGenericText(PboEntry entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetText());
        }

        private void ShowText(string text)
        {
            ExtractFileText.IsEnabled = true;
            TextPreview.Text = text;
            TextPreview.Visibility = Visibility.Visible;
        }

        private void ShowDetectConfig(PboEntry entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetDetectConfigAsText(out bool wasBinary));
            infos.Add(new PropertyItem("Format", wasBinary ? "Binarized" : "Text"));
        }

        private void ShowGenericBinary(PboEntry entry, List<PropertyItem> infos)
        {
            if (string.Equals(entry.Name, "config.bin", StringComparison.OrdinalIgnoreCase))
            {
                ShowBinaryConfig(entry, infos);
                return;
            }
        }

        private void ShowBinaryConfig(PboEntry entry, List<PropertyItem> infos)
        {
            ShowText(entry.GetBinaryConfigAsText());
        }

        private void ShowImage(PboEntry entry, List<PropertyItem> infos)
        {

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
    }
}
