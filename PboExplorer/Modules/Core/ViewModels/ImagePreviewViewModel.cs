using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PboExplorer.Modules.Core.ViewModels;

public class ImagePreviewViewModel:Document
{
    public ImageSource Image { get; }

	public ImagePreviewViewModel(string name, ImageSource image)
	{
		DisplayName = name;
		Image = image;
	}
}
