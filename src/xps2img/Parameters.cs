using System.Drawing;

namespace xps2img
{
    public class Parameters
    {
        public ImageType ImageType { get; set; }
        public ImageOptions ImageOptions { get; set; }
        public Size? RequiredSize { get; set; }
        public int Dpi { get; set; }
    }
}