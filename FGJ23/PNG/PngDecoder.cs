using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace FGJ23.PNG
{
    static class PngDecoder
    {
        public static bool[,] DecodeMaskFromPng(byte[] data)
        {
            List<bool[]> res = new();
            // Convert it to a transparent png...
            var pngData = ReadImageToRgbaPng(data);
            // and then handle normally
            using (Image<Rgba32> image = Image.Load(pngData))
            {
                bool[] row = new bool[image.Width];
                Log.Information("Width: {A}, Height: {B}", image.Width, image.Height);
                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);
                    for (int x = 0; x < image.Width; x++)
                    {
                        var color = pixelRowSpan[x];
                        row[x] = color.A > 0;
                    }
                    res.Add(row.Clone() as bool[]);
                }
            }

            bool[,] arrRes = new bool[res.Count, res[0].Length];

            for (int y = 0; y < res.Count; y++)
            {
                for (int x = 0; x < res[y].Length; x++)
                {
                    arrRes[y, x] = res[y][x];
                }
            }

            return arrRes;
        }

        public static byte[] ReadImageToRgbaPng(byte[] data)
        {
            Image meta = Image.Load(data);
            if (meta.Metadata.GetPngMetadata().ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.Palette)
            {
                var rgbaData = ImageResult.FromIndexedWithFirstColorBeingTransparent(data);
                var stream = new MemoryStream();
                using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(rgbaData.Data, meta.Width, meta.Height))
                {
                    image.SaveAsPng(stream);
                }
                return stream.GetBuffer();
            }
            else
            {
                return data;
            }
        }
    }
}
