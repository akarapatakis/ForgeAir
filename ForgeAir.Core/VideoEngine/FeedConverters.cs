using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;

namespace ForgeAir.Core.VideoEngine
{
    public class FeedConverters
    {
        public static Bitmap RenderToBitmap(UIElement element, int width, int height)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var dpi = 96d;
            var renderTarget = new RenderTargetBitmap(
                width, height,
                dpi, dpi,
                PixelFormats.Pbgra32
            );

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                var vb = new VisualBrush(element);
                context.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(width, height)));
            }

            renderTarget.Render(visual);

            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var data = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            renderTarget.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,
                data.Stride * height,
                data.Stride
            );

            bitmap.UnlockBits(data);
            return bitmap;
        }
    }
}
