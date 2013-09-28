using System.IO;
using System.Linq;
using NUnit.Framework;

namespace xps2img.tests
{
    [TestFixture, RequiresSTA]
    public class Xps2ImageTests
    {
        private readonly string _xpsSinglePage = Path.ChangeExtension(Path.GetTempFileName(), ".xps");
        private readonly string _xpsMultiplePages = Path.ChangeExtension(Path.GetTempFileName(), ".xps");

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            if (File.Exists(_xpsSinglePage))
            {
                File.Delete(_xpsSinglePage);
            }
            if (File.Exists(_xpsMultiplePages))
            {
                File.Delete(_xpsMultiplePages);
            }
            File.WriteAllBytes(_xpsSinglePage, Properties.Resources.singlepage);
            File.WriteAllBytes(_xpsMultiplePages, Properties.Resources.multiplepages);
        }

        [Test]
        public void TestConvertByteArray()
        {
            using (var xpsConverter = new Xps2Image(File.ReadAllBytes(_xpsSinglePage)))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),                    
                });

                Assert.That(images.Count(), Is.EqualTo(1));
            }
            using (var xpsConverter = new Xps2Image(File.ReadAllBytes(_xpsMultiplePages)))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
                });

                Assert.That(images.Count(), Is.EqualTo(5));
            }
        }

        [Test]
        public void TestConvertStream()
        {
            using (var xpsConverter = new Xps2Image(new MemoryStream(File.ReadAllBytes(_xpsSinglePage))))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
                });

                Assert.That(images.Count(), Is.EqualTo(1));
            }
            using (var xpsConverter = new Xps2Image(new MemoryStream(File.ReadAllBytes(_xpsMultiplePages))))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
                });

                Assert.That(images.Count(), Is.EqualTo(5));
            }
        }

        [Test]
        public void TestConvertFromDisk()
        {
            using (var xpsConverter = new Xps2Image(_xpsSinglePage))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
                });

                Assert.That(images.Count(), Is.EqualTo(1));
            }
            using (var xpsConverter = new Xps2Image(_xpsMultiplePages))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300,
                    ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
                });

                Assert.That(images.Count(), Is.EqualTo(5));
            }
        }

        [Test]
        public void TestStaticConvertByteArray()
        {
            var images = Xps2Image.ToBitmap(File.ReadAllBytes(_xpsSinglePage), new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(1));

            images = Xps2Image.ToBitmap(File.ReadAllBytes(_xpsMultiplePages), new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(5));
        }

        [Test]
        public void TestStaticConvertStream()
        {
            var images = Xps2Image.ToBitmap(new MemoryStream(File.ReadAllBytes(_xpsSinglePage)), new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(1));

            images = Xps2Image.ToBitmap(new MemoryStream(File.ReadAllBytes(_xpsMultiplePages)), new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(5));
        }

        [Test]
        public void TestStaticConvertFromDisk()
        {
            var images = Xps2Image.ToBitmap(_xpsSinglePage, new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(1));

            images = Xps2Image.ToBitmap(_xpsMultiplePages, new Parameters
            {
                ImageType = ImageType.Png,
                Dpi = 300,
                ImageOptions = new ImageOptions(100, TiffCompressOption.Zip),
            });

            Assert.That(images.Count(), Is.EqualTo(5));
        }

    }
}
