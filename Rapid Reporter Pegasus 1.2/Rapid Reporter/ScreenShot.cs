using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Code adapted from:
// http://kseesharp.blogspot.com/2008/11/c-capture-screenshot.html
// Sent email requesting license to adapt the code and permission to refer to his link.
//  There were many changes to the code, but still good to keep credit for original.
//  Althought similar code appears in so many places thet it is hard to point who is the original. Probably Microsoft :).

namespace Rapid_Reporter
{
    class ScreenShot
    {
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);

        public Bitmap CaptureScreenShot()
        {
            Logger.record("[CaptureScreenshot]: Will take a screenshot of the monitor.", "ScreenShot", "info");
            Rectangle fullBounds = new Rectangle();
            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            /*

             * Composite desktop calculations for multiple monitors

                A, B            E, B                     ||                  E, B
                  +--------------+                       ||   *              +---------------+ D, B
                  |              |E, G                   ||                  |               |
                  |              +---------------+ D, G  ||   +--------------+ E, C          |
                  |              |               |       ||   |A, C          |               |
                  |              |               |       ||   |              |               |
                  |              |               |       ||   |              |               |
                  +--------------+ E, C          |       ||   |              +---------------+ D, G
                A, C             |               |       ||   |              |E, G
                                 +---------------+ D, F  ||   +--------------+               *
                                E, F                     ||   A, F           E, F

             * We capture from "A, B" to "D, F".
             *   That is, we look for the minimum X,Y coordinate first.
             *   Then we look for the largest width,height.
 
            */

            foreach (Screen oneScreen in Screen.AllScreens)
            {
                Logger.record("\t[CaptureScreenshot]: AllScreens[]: " + oneScreen.Bounds.ToString(), "ScreenShot", "info");
                minX = Math.Min(minX, oneScreen.Bounds.Left);
                minY = Math.Min(minY, oneScreen.Bounds.Top);
                maxX = Math.Max(maxX, oneScreen.Bounds.Width + oneScreen.Bounds.X);
                maxY = Math.Max(maxY, oneScreen.Bounds.Height + oneScreen.Bounds.Y);
            }
            fullBounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            Logger.record("[CaptureScreenshot]: fullScreen[]: " + fullBounds.ToString(), "ScreenShot", "info");

            IntPtr hDesk = GetDesktopWindow();
            IntPtr hSrce = GetWindowDC(hDesk);
            IntPtr hDest = CreateCompatibleDC(hSrce);
            // Our bitmap will have the size of the composite screenshot
            IntPtr hBmp = CreateCompatibleBitmap(hSrce, fullBounds.Width, fullBounds.Height);
            IntPtr hOldBmp = SelectObject(hDest, hBmp);
            // We write on coordinate 0,0 of the bitmap buffer, of course. But we write the the fullBoundsX,Y pixels.
            bool b = BitBlt(hDest, 0, 0, fullBounds.Width, fullBounds.Height, hSrce, fullBounds.X, fullBounds.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Bitmap.FromHbitmap(hBmp);
            SelectObject(hDest, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(hDest);
            ReleaseDC(hDesk, hSrce);
            Logger.record("[CaptureScreenshot]: BMP object ready, returning it to calling function", "ScreenShot", "info");
            return (bmp);
        }
    }
}