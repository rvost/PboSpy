using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.ViewModels;

public class TextPreviewViewModel : Document
{
    public string Text { get; }

	public TextPreviewViewModel(string name, string text)
	{
		DisplayName= name;
		Text= text;
	}
}
