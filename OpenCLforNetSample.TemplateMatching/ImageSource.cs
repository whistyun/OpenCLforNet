using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenCLforNetSample.TemplateMatching
{
    public class ImageSource
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] Data { get; }

        public ImageSource(Bitmap bmp)
        {
            Width = bmp.Width;
            Height = bmp.Height;

            BitmapData bmpData =
                bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

            try
            {
                int bytes = bmp.Width * bmp.Height * 3;
                Data = new byte[bytes];

                IntPtr ptr = bmpData.Scan0;
                for (int i = 0; i < Data.Length; i += bmp.Width * 3)
                {
                    Marshal.Copy(ptr, Data, i, bmp.Width * 3);
                    ptr += bmpData.Stride;
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }

        public static ImageSource Create(Bitmap bmp, int widAdd, int heiAdd)
        {
            if (widAdd == 0 && heiAdd == 0)
            {
                return new ImageSource(bmp);
            }
            else
            {
                Bitmap bmp2 = new Bitmap(
                        bmp.Width + widAdd,
                        bmp.Height + heiAdd,
                        PixelFormat.Format24bppRgb);

                using (var g = Graphics.FromImage(bmp2))
                {
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                }

                return new ImageSource(bmp2);
            }
        }
    }
}
