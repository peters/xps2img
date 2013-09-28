using System.ComponentModel;

namespace xps2img
{
    public enum ImageType
    {
        Png,
        Jpeg,
        Tiff,
        Bmp,
        Gif
    }

    public enum TiffCompressOption
    {
        Default,
        None,
        [Description("CCITT3")]
        Ccitt3,
        [Description("CCITT4")]
        Ccitt4,
        [Description("LZW")]
        Lzw,
        [Description("RLE")]
        Rle,
        [Description("ZIP")]
        Zip
    }

    public class ImageOptions
    {
        public readonly int JpegQualityLevel;
        public readonly TiffCompressOption TiffCompression;

        public static readonly ImageOptions Default = new ImageOptions(85, TiffCompressOption.Zip);

        public ImageOptions()
            : this(Default.JpegQualityLevel, Default.TiffCompression)
        {
        }

        public ImageOptions(int jpegQualityLevel, TiffCompressOption tiffCompression)
        {
            JpegQualityLevel = jpegQualityLevel;
            TiffCompression = tiffCompression;
        }

    }
}
