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
        Ccitt3,
        Ccitt4,
        Lzw,
        Rle,
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
