using Net.Codecrete.QrCodeGenerator;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace BotBiliBili.Utils;

public static class QrCodeBitmap
{
    /// <inheritdoc cref="ToBitmap(QrCode, int, int)"/>
    /// <param name="background">The background color.</param>
    /// <param name="foreground">The foreground color.</param>
    public static Image ToBitmap(string text, Color foreground, Color background, int scale = 20, int border = 2)
    {
        var qr = QrCode.EncodeText(text, QrCode.Ecc.Quartile);

        // check arguments
        if (scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
        }
        if (border < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
        }

        int size = qr.Size;
        int dim = (size + border * 2) * scale;

        if (dim > short.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
        }

        // create bitmap
        Image<Rgba32> image = new(dim, dim);

        image.Mutate(img =>
        {
            // draw background
            img.Fill(background);

            // draw modules
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (qr.GetModule(x, y))
                    {
                        img.Fill(foreground, new Rectangle((x + border) * scale, (y + border) * scale, scale, scale));
                    }
                }
            }
        });

        return image;
    }
}