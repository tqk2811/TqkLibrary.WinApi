using System;
using System.Drawing;
using TqkLibrary.WinApi.Enums;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        //https://stackoverflow.com/questions/5069104/fastest-method-of-screen-capturing-on-windows
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPtr">Window handle</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bitmap? Capture(this IntPtr intPtr, CaptureType type = CaptureType.PrintWindow)
        {
            if (PInvoke.GetWindowRect((HWND)intPtr, out RECT windowRect))// get the size
            {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;
                switch (type)
                {
                    case CaptureType.PrintWindow:
                        {
                            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            using Graphics fgx = Graphics.FromImage(bitmap);
                            IntPtr hdc = fgx.GetHdc();
                            BOOL flag = PInvoke.PrintWindow((HWND)intPtr, (HDC)hdc, (PRINT_WINDOW_FLAGS)0);
                            fgx.ReleaseHdc(hdc);
                            if (flag) return bitmap;
                            else
                            {
                                using (var bm = bitmap) return null;
                            }
                        }

                    case CaptureType.BitBlt:
                        {
                            // get te hDC of the target window
                            HDC hdcSrc = default;
                            HDC hdcDest = default;
                            HBITMAP hBitmap = default;
                            HGDIOBJ hOld = default;
                            try
                            {
                                hdcSrc = PInvoke.GetWindowDC((HWND)intPtr);
                                if (hdcSrc == 0)
                                    return null;

                                // create a device context we can copy to
                                hdcDest = PInvoke.CreateCompatibleDC(hdcSrc);
                                if (hdcDest == 0)
                                    return null;

                                // create a bitmap we can copy it to,
                                hBitmap = PInvoke.CreateCompatibleBitmap(hdcSrc, width, height);
                                // select the bitmap object
                                hOld = PInvoke.SelectObject(hdcDest, hBitmap);

                                // bitblt over
                                BOOL result = PInvoke.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, ROP_CODE.SRCCOPY);

                                if (result)
                                {
                                    IntPtr obj = PInvoke.SelectObject(hdcDest, hOld);
                                    return Image.FromHbitmap(hBitmap);
                                }
                                else return null;
                            }
                            finally
                            {
                                if (hBitmap != default) PInvoke.DeleteObject(hBitmap);
                                if (hdcDest != default) PInvoke.DeleteDC(hdcDest);
                                if (hdcSrc != default) PInvoke.ReleaseDC((HWND)intPtr, hdcSrc);
                            }
                        }

                        //case ScreenShotType.DesktopDuplicationAPI:
                        //    {

                        //        break;
                        //    }
                }
            }
            return null;
        }
    }
}
