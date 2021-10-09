using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BIS.Core.Config;
using BIS.PAA;
using BIS.PBO;

namespace PboExplorer
{
    class PboEntry : ITreeItem
    {
        private readonly PBO pbo;

        public PboEntry(PBO pbo, FileEntry entry)
        {
            this.pbo = pbo;
            Name = Path.GetFileName(entry.FileName);
            Extension = Path.GetExtension(entry.FileName).ToLowerInvariant();
            Entry = entry;
        }

        public PBO PBO => pbo;

        public FileEntry Entry { get; }

        public string Name { get; }

        public string Extension { get; }

        public ICollection<ITreeItem> Children => null;

        public string FullPath => pbo.Prefix + "\\" + Entry.FileName;

        public Stream GetStream()
        {
            return pbo.GetFileEntryStream(Entry);
        }

        public string GetText()
        {
            using(var reader = new StreamReader(GetStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public string GetBinaryConfigAsText()
        {
            return GetBinaryConfig().ToString();

        }
        public ParamFile GetBinaryConfig()
        {
            using (var stream = GetStream())
            {
                return new ParamFile(stream);
            }
        }

        public PaaImage GetPaaImage()
        {
            using (var paaStream = GetStream())
            {
                var paa = new PAA(paaStream, Extension == ".pac");
                var pixels = PAA.GetARGB32PixelData(paa, paaStream);
                var colors = paa.Palette.Colors.Select(c => Color.FromRgb(c.R8, c.G8, c.B8)).ToList();
                var bitmapPalette = (colors.Count > 0) ? new BitmapPalette(colors) : null;
                var bms = BitmapSource.Create(paa.Width, paa.Height, 300, 300, PixelFormats.Bgra32, bitmapPalette, pixels, paa.Width * 4);

                return new PaaImage()
                {
                    Paa = paa,
                    Bitmap = bms
                };
            }
        }

        public string GetDetectConfigAsText(out bool wasBinary)
        {
            using (var stream = GetStream())
            {
                var buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                stream.Seek(0, SeekOrigin.Begin);
                if (buffer.SequenceEqual(new byte[] { 0, (byte)'r', (byte)'a', (byte)'P' }))
                {
                    wasBinary = true;
                    return new ParamFile(stream).ToString();
                }
                wasBinary = false;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal void Extract(string fileName)
        {
            using (var stream = File.Create(fileName))
            {
                pbo.GetFileEntryStream(Entry).CopyTo(stream);
            }
        }
    }
}
