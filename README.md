[xps2im](https://www.nuget.org/packages/xps2img/)
=========

Convert xps documents to a .NET bitmap. Supports both single and multi-paged xps documents.

This code is mostly derived from [xps2img@sourceforge](http://sourceforge.net/projects/xps2img/) by Ivan Ivon.

Installing via Nuget
====================

	Install-Package xps2img

Supported frameworks
====================

1. .NET Framework 4.0
2. .NET Framework 4.5
3. .NET Framework 4.5.1

Usage
=====

[Additional examples](https://github.com/peters/xps2image/blob/master/src/xps2img.tests/Xps2ImageTests.cs)

**Convert a xps document to png from disk**

```cs
using (var xpsConverter = new Xps2Image("multipage.xps"))
{
	var images = xpsConverter.ToBitmap(new Parameters
	{
		ImageType = ImageType.Png,
		Dpi = 300
	});
}
```

**Convert a xps document to png from a byte array**

```cs
using (var xpsConverter = new Xps2Image(File.ReadAllBytes("multipage.xps")))
{
	var images = xpsConverter.ToBitmap(new Parameters
	{
		ImageType = ImageType.Png,
		Dpi = 300
	});
}
```

**Convert a xps document to png from stream**

```cs
using (var xpsConverter = new Xps2Image(new MemoryStream(File.ReadAllBytes("multipage.xps"))))
{
	var images = xpsConverter.ToBitmap(new Parameters
	{
		ImageType = ImageType.Png,
		Dpi = 300
	});
}
```

License
=======

[GNU LESSER GENERAL PUBLIC LICENSE Version 3](https://github.com/peters/xps2img/blob/master/LICENSE)
