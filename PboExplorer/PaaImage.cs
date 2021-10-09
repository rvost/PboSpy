using System.Windows.Media.Imaging;
using BIS.PAA;

namespace PboExplorer
{
    internal class PaaImage
    {
        public PaaImage()
        {
        }

        public PAA Paa { get; set; }
        public BitmapSource Bitmap { get; set; }
    }
}