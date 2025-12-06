using System.IO;
using Avalonia.Media;

namespace ForgeAir.Playout.App.Helpers
{
    public static class ImageHelper
    {
        public static IImage ByteArrayToImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            return new Avalonia.Media.Imaging.Bitmap(new MemoryStream(data));
        }

        public static IImage LoadBitmap(string path)
        {
            return new Avalonia.Media.Imaging.Bitmap(path);
        }



    }
}
