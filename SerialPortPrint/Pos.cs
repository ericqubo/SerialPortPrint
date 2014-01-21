using System;
using System.Collections.Generic;

#if ANDROID
using Android.Graphics;
#else
using System.Drawing;
using System.Drawing.Drawing2D;
#endif

using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SerialPortPrint
{
    /// <summary>
    /// 图片转打印命令
    /// </summary>
    public class Pos
    {
        private static int[] p0 = { 0, 128 };
        private static int[] p1 = { 0, 64 };
        private static int[] p2 = { 0, 32 };
        private static int[] p3 = { 0, 16 };
        private static int[] p4 = { 0, 8 };
        private static int[] p5 = { 0, 4 };
        private static int[] p6 = { 0, 2 };

        public static byte[] POS_PrintPicture(Bitmap mBitmap, int nWidth,
            int nMode)
        {

            int width = (nWidth + 7) / 8 * 8;

            int height = mBitmap.Height * width / mBitmap.Width;

            //Bitmap grayBitmap = toGrayscale(mBitmap);

            Bitmap rszBitmap = resizeImage(mBitmap, width, height);

            byte[] src = bitmapToBWPix(rszBitmap);
            byte[] data = pixToCmd(src, width, nMode);

            rszBitmap.Dispose();
            rszBitmap = null;

            return data;

        }


        public static Bitmap resizeImage(Bitmap bitmap, int w, int h)
        {
            Bitmap bitmapOrg = bitmap;

            int width = bitmapOrg.Width;
            int height = bitmapOrg.Height;
            int newWidth = w;
            int newHeight = h;

            float scaleWidth = newWidth / width;
            float scaleHeight = newHeight / height;


#if ANDROID
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

            Matrix matrix = new Matrix();
            matrix.PostScale(scaleWidth, scaleHeight);// 产生缩放后的Bitmap对象

            Bitmap bmp = Bitmap.CreateBitmap(bitmapOrg, 0, 0, width, height, matrix, false);
            bitmapOrg.Recycle();
            bitmapOrg = null;

           
#else
            Bitmap bmp = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(bmp);
            
            Rectangle rect = new Rectangle(0, 0, newWidth, newHeight);
            g.FillRectangle(Brushes.White, rect);

            // 改变图像大小使用低质量的模式 
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(bitmapOrg, new Rectangle(0, 0, newWidth, newWidth), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            
            bitmapOrg.Dispose();
            bitmapOrg = null;
            //bmp.Save(newImg);
#endif



            /*  
                           // 使用高质量模式 
                           g.CompositingQuality = CompositingQuality.HighSpeed; 
                           g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                       g.DrawImage( bmp, new Rectangle(130, 10, 120, 120), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                         */
            return bmp;
        }

        public static byte[] bitmapToBWPix(Bitmap mBitmap)
        {

            int[] pixels = new int[mBitmap.Width * mBitmap.Height];

            byte[] data = new byte[mBitmap.Width * mBitmap.Height];

            Bitmap grayBitmap = mBitmap; //toGrayscale(mBitmap);

            pixels = RGB2Gray(grayBitmap);

            format_K_dither16x16(pixels, grayBitmap.Width,
                grayBitmap.Height, data);

            return data;
            /*     */
        }

        public static int[] RGB2Gray(Bitmap srcBitmap)
        {            
            int wide = srcBitmap.Width;
            int height = srcBitmap.Height;
            int[] regIng = new int[wide * height];
            int index=0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < wide; x++)
                {
#if ANDROID
                    regIng[index] = srcBitmap.GetPixel(x, y);
#else
                    //获取像素的ＲＧＢ颜色值
                    regIng[index] = srcBitmap.GetPixel(x, y).ToArgb(); //GetCustomColor(srcColor);
#endif
                    index++;
                    
                }
            return regIng;
        }

        private static uint GetCustomColor(Color color)
        {
            int nColor = color.ToArgb();

            int blue = nColor & 255;

            int green = nColor >> 8 & 255;

            int red = nColor >> 16 & 255;

            return Convert.ToUInt32(blue << 16 | green << 8 | red);
        }


        private static void format_K_dither16x16(int[] orgpixels, int xsize, int ysize, byte[] despixels)
        {
            int k = 0;
            for (int y = 0; y < ysize; y++)
            {
                for (int x = 0; x < xsize; x++)
                {
                    if ((orgpixels[k] & 0xFF) > Floyd16x16[(x & 0xF)][(y & 0xF)])
                        despixels[k] = 0;
                    else
                    {
                        despixels[k] = 1;
                    }
                    k++;
                }
            }
        }

        private static byte[] pixToCmd(byte[] src, int nWidth, int nMode)
        {
            int nHeight = src.Length / nWidth;
            byte[] data = new byte[8 + src.Length / 8];
            data[0] = 29;
            data[1] = 118;
            data[2] = 48;
            data[3] = (byte)(nMode & 0x1);
            data[4] = (byte)(nWidth / 8 % 256);
            data[5] = (byte)(nWidth / 8 / 256);
            data[6] = (byte)(nHeight % 256);
            data[7] = (byte)(nHeight / 256);
            int k = 0;
            for (int i = 8; i < data.Length; i++)
            {
                data[i] =
                  (byte)(p0[src[k]] + p1[src[(k + 1)]] + p2[src[(k + 2)]] +
                  p3[src[(k + 3)]] + p4[src[(k + 4)]] + p5[src[(k + 5)]] +
                  p6[src[(k + 6)]] + src[(k + 7)]);
                k += 8;
            }
            return data;
        }


        private static int[][] Floyd16x16 = {
				new int[] { 0, 128, 32, 160, 8, 136, 40, 168, 2, 130, 34, 162, 10,
						138, 42, 170 },
				new int[] { 192, 64, 224, 96, 200, 72, 232, 104, 194, 66, 226, 98,
						202, 74, 234, 106 },
				new int[] { 48, 176, 16, 144, 56, 184, 24, 152, 50, 178, 18, 146,
						58, 186, 26, 154 },
				new int[] { 240, 112, 208, 80, 248, 120, 216, 88, 242, 114, 210,
						82, 250, 122, 218, 90 },
				new int[] { 12, 140, 44, 172, 4, 132, 36, 164, 14, 142, 46, 174, 6,
						134, 38, 166 },
				new int[] { 204, 76, 236, 108, 196, 68, 228, 100, 206, 78, 238,
						110, 198, 70, 230, 102 },
				new int[] { 60, 188, 28, 156, 52, 180, 20, 148, 62, 190, 30, 158,
						54, 182, 22, 150 },
				new int[] { 252, 124, 220, 92, 244, 116, 212, 84, 254, 126, 222,
						94, 246, 118, 214, 86 },
				new int[] { 3, 131, 35, 163, 11, 139, 43, 171, 1, 129, 33, 161, 9,
						137, 41, 169 },
				new int[] { 195, 67, 227, 99, 203, 75, 235, 107, 193, 65, 225, 97,
						201, 73, 233, 105 },
				new int[] { 51, 179, 19, 147, 59, 187, 27, 155, 49, 177, 17, 145,
						57, 185, 25, 153 },
				new int[] { 243, 115, 211, 83, 251, 123, 219, 91, 241, 113, 209,
						81, 249, 121, 217, 89 },
				new int[] { 15, 143, 47, 175, 7, 135, 39, 167, 13, 141, 45, 173, 5,
						133, 37, 165 },
				new int[] { 207, 79, 239, 111, 199, 71, 231, 103, 205, 77, 237,
						109, 197, 69, 229, 101 },
				new int[] { 63, 191, 31, 159, 55, 183, 23, 151, 61, 189, 29, 157,
						53, 181, 21, 149 },
				new int[] { 254, 127, 223, 95, 247, 119, 215, 87, 253, 125, 221,
						93, 245, 117, 213, 85 }
                                            
                                            };
    }
}