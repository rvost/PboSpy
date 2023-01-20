using Microsoft.Win32;
using PboExplorer.Models;
using PboExplorer.Modules.Extraction.Commands;
using PboExplorer.Modules.Preview.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PboExplorer.Modules.PreviewImage.ViewModels;

public class ImagePreviewViewModel : PreviewViewModel, ICommandHandler<ExtractAsPngCommandDefinition>
{
    public ImageSource Image { get; }

    public ImagePreviewViewModel(FileBase model, ImageSource image) : base(model)
    {
        Image = image;
        Image.Freeze();
    }

    protected override void CanExecuteCopy(Command command)
    {
        command.Enabled = Image is BitmapSource;
    }

    protected override Task ExecuteCopy(Command command)
    {
        if (Image is BitmapSource bmp)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using var pngMemStream = new MemoryStream();

            encoder.Save(pngMemStream);
            var data = new DataObject();
            data.SetImage(bmp); // For applications that does not support PNG data
            data.SetData("PNG", pngMemStream, false);
            Clipboard.SetDataObject(data, true);
        }
        return Task.CompletedTask;
    }

    void ICommandHandler<ExtractAsPngCommandDefinition>.Update(Command command)
    {
        command.Enabled = Image is BitmapSource;
    }

    Task ICommandHandler<ExtractAsPngCommandDefinition>.Run(Command command)
    {
        if (Image is BitmapSource bmp)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Extract to PNG",
                FileName = Path.ChangeExtension(_model.Name, ".png"),
                DefaultExt = ".png",
                Filter = "PNG|*.png"
            };

            if (dlg.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using var stream = File.Create(dlg.FileName);
                encoder.Save(stream);
            }
        }

        return Task.CompletedTask;
    }

}
