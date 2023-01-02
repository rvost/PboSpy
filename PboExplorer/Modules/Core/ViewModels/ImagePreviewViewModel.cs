using Gemini.Framework.Commands;
using PboExplorer.Modules.Core.Commands;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Move ICommandHandler<CopyToClipboardCommandDefinition> to base class
public class ImagePreviewViewModel : PreviewViewModel, ICommandHandler<CopyToClipboardCommandDefinition>
{
    public ImageSource Image { get; }

    public ImagePreviewViewModel(FileBase model, ImageSource image) : base(model)
    {
        Image = image;
    }

    void ICommandHandler<CopyToClipboardCommandDefinition>.Update(Command command)
    {
        command.Enabled = Image is BitmapSource;
    }

    Task ICommandHandler<CopyToClipboardCommandDefinition>.Run(Command command)
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
}
