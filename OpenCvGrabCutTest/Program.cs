using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvGrabCutTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GrabCutWithRect();
            GrabCutWithMask();

            Cv2.WaitKey();
        }

        private static void GrabCutWithRect()
        {
            using (var img = new Mat(@"Images/1.jpg"))
            using (var mask = new Mat(img.Size(), MatType.CV_8UC3))
            {
                // 領域指定でGrabCutを実行
                var bmModel = new Mat();
                var fgModel = new Mat();
                Cv2.GrabCut(img, mask, new Rect(115, 20, 630, 625), bmModel, fgModel, 5, GrabCutModes.InitWithRect);

                // 前景(1)/おそらく前景(3)とラベリングされた部分⇒1、それ以外は0のマスク画像を作る
                var lut2 = new byte[256];
                lut2[0] = 0; lut2[1] = 1; lut2[2] = 0; lut2[3] = 1;
                Cv2.LUT(mask, lut2, mask);

                // 前景部分を抽出
                var foreground = new Mat(img.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0));
                img.CopyTo(foreground, mask);
                Cv2.ImShow("GrabCut(InitWithRect)", foreground);
            }
        }

        private static void GrabCutWithMask()
        {
            using (var img = new Mat(@"Images/1.jpg"))
            using (var mask = new Mat(@"Images/1_mask.png", ImreadModes.GrayScale))
            {
                var lut = Enumerable.Repeat<byte>(2, 256)
                                    .ToArray();
                // マスク画像として用意したbmpの各ピクセル値を以下のように変換する
                // 黒(0)      ⇒おそらく背景(2)
                // 白(255)    ⇒おそらく前景(3)
                lut[0] = (byte)2;
                lut[255] = (byte)3;
                Cv2.LUT(mask, lut, mask);

                // マスク画像を用いてGrabCutを実行
                var bmModel = new Mat();
                var fgModel = new Mat();
                Cv2.GrabCut(img, mask, Rect.Empty, bmModel, fgModel, 5, GrabCutModes.InitWithMask);

                // 前景(1)/おそらく前景(3)とラベリングされた部分⇒1、それ以外は0のマスク画像を作る
                var lut2 = new byte[256];
                lut2[0] = 0; lut2[1] = 1; lut2[2] = 0; lut2[3] = 1;
                Cv2.LUT(mask, lut2, mask);

                // 前景部分を抽出
                var foreground = new Mat(img.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0));
                img.CopyTo(foreground, mask);
                Cv2.ImShow("GrabCut(InitWithMask)", foreground);
            }
        }
    }
}
