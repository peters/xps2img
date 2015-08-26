using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace xps2img
{
    // copyright: http://sourceforge.net/projects/xps2img/

    public class Xps2Image : IDisposable
    {
        private XpsDocument _xpsDocument;
        private DocumentPaginator _xpsDocumentPaginator;
        private MemoryStream _xpsDocumentInMemoryStream = new MemoryStream();

        public int PageCount
        {
            get
            {
                return _xpsDocumentPaginator.PageCount;
            }
        }

        public System.Windows.Size PageSize
        {
            get
            {
                return _xpsDocumentPaginator.PageSize;
            }
        }

        public Xps2Image(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                _xpsDocumentInMemoryStream.Write(bytes, 0, (int)file.Length);
            }
        }

        public Xps2Image(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            stream.CopyTo(_xpsDocumentInMemoryStream);
        }

        public Xps2Image(byte[] byteArray)
        {
            _xpsDocumentInMemoryStream.Write(byteArray, 0, byteArray.Length);
        }

        public IEnumerable<Bitmap> ToBitmap(Parameters parameters)
        {
            var pages = new List<Bitmap>();
            var thread = new Thread(() =>
            {
                const string inMemoryPackageName = "memorystream://inmemory.xps";
                var packageUri = new Uri(inMemoryPackageName);
                using (var package = Package.Open(_xpsDocumentInMemoryStream))
                {
                    PackageStore.AddPackage(packageUri, package);

                    _xpsDocument = new XpsDocument(package, CompressionOption.Normal, inMemoryPackageName);
                    _xpsDocumentPaginator = _xpsDocument.GetFixedDocumentSequence().DocumentPaginator;

                    for (var docPageNumber = 0; docPageNumber < PageCount; docPageNumber++)
                    {
                        pages.Add(ProcessPage(parameters, docPageNumber));
                    }

                    PackageStore.RemovePackage(packageUri);

                    _xpsDocument.Close();
                    _xpsDocument = null;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return pages;
        }

        public static IEnumerable<Bitmap> ToBitmap(string filename, Parameters parameters)
        {
            using (var xpsConverter = new Xps2Image(filename))
            {
                return xpsConverter.ToBitmap(parameters).ToList();
            }
        }

        public static IEnumerable<Bitmap> ToBitmap(byte[] byteArray, Parameters parameters)
        {
            using (var xpsConverter = new Xps2Image(byteArray))
            {
                return xpsConverter.ToBitmap(parameters).ToList();
            }
        }

        public static IEnumerable<Bitmap> ToBitmap(Stream stream, Parameters parameters)
        {
            using (var xpsConverter = new Xps2Image(stream))
            {
                return xpsConverter.ToBitmap(parameters).ToList();
            }
        }

        public static IEnumerable<Bitmap> ToBitmap(IEnumerable<string> filenames, Parameters parameters)
        {
            foreach (var filename in filenames)
            {
                using (var xpsConverter = new Xps2Image(filename))
                {
                    return xpsConverter.ToBitmap(parameters).ToList();
                }
            }
            return new List<Bitmap>();
        }

        public static IEnumerable<Bitmap> ToBitmap(IEnumerable<Stream> streams, Parameters parameters)
        {
            foreach (var stream in streams)
            {
                using (var xpsConverter = new Xps2Image(stream))
                {
                    return xpsConverter.ToBitmap(parameters).ToList();
                }
            }
            return new List<Bitmap>();
        }

        public static IEnumerable<Bitmap> ToBitmap(IEnumerable<byte[]> byteArrays, Parameters parameters)
        {
            foreach (var byteArray in byteArrays)
            {
                using (var xpsConverter = new Xps2Image(byteArray))
                {
                    return xpsConverter.ToBitmap(parameters).ToList();
                }
            }
            return new List<Bitmap>();
        }

        private Bitmap ProcessPage(Parameters parameters, int docPageNumber)
        {
            var bitmapEncoder = CreateEncoder(parameters.ImageType, parameters.ImageOptions);
            var bitmapSource = GetPageBitmap(_xpsDocumentPaginator, docPageNumber, parameters);

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(stream);

                return new Bitmap(stream);
            }
        }

        private RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, Parameters parameters)
        {
            const double dpiConst = 96.0;

            double dpi = parameters.Dpi;

            var size = parameters.RequiredSize ?? new Size();

            Func<int, bool> isSizeDefined = requiredSize => requiredSize > 0;
            Action<int, double> calcDpi = (requiredSize, pageSize) =>
            {
                if (isSizeDefined(requiredSize))
                {
                    dpi = (requiredSize / pageSize) * dpiConst;
                }
            };

            try
            {
                using (var page = documentPaginator.GetPage(pageNumber))
                {
                    if (!size.IsEmpty)
                    {
                        var portrait = page.Size.Height >= page.Size.Width;

                        if (portrait || !isSizeDefined(size.Width))
                        {
                            calcDpi(size.Height, page.Size.Height);
                        }

                        if (!portrait || !isSizeDefined(size.Height))
                        {
                            calcDpi(size.Width, page.Size.Width);
                        }
                    }

                    var ratio = dpi / dpiConst;

                    var bitmap = new RenderTargetBitmap((int)Math.Round(page.Size.Width * ratio),
                                                        (int)Math.Round(page.Size.Height * ratio), dpi, dpi, PixelFormats.Pbgra32);

                    bitmap.Render(page.Visual);

                    // Memory leak fix.
                    // http://social.msdn.microsoft.com/Forums/en/wpf/thread/c6511918-17f6-42be-ac4c-459eeac676fd
                    ((FixedPage)page.Visual).UpdateLayout();

                    return bitmap;

                }
            }
            catch (XamlParseException ex)
            {
                throw new ConversionException(ex.Message, pageNumber + 1, ex);
            }
        }

        private static BitmapEncoder CreateEncoder(ImageType imageType, ImageOptions imageOptions)
        {
            imageOptions = imageOptions ?? ImageOptions.Default;

            switch (imageType)
            {
                case ImageType.Png:
                    return new PngBitmapEncoder();
                case ImageType.Jpeg:
                    return new JpegBitmapEncoder
                    {
                        QualityLevel = imageOptions.JpegQualityLevel
                    };
                case ImageType.Tiff:
                    return new TiffBitmapEncoder
                    {
                        Compression = (System.Windows.Media.Imaging.TiffCompressOption)imageOptions.TiffCompression
                    };
                case ImageType.Bmp:
                    return new BmpBitmapEncoder();
                case ImageType.Gif:
                    return new GifBitmapEncoder();
                default:
                    throw new InvalidOperationException();
            }
        }

        public void Dispose()
        {
            _xpsDocumentInMemoryStream.Dispose();
            _xpsDocumentInMemoryStream = null;
            GC.SuppressFinalize(this);
        }
    }
}
