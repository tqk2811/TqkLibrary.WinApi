using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public enum ScreenShotType
    {
        /// <summary>
        /// 
        /// </summary>
        PrintWindow,
        /// <summary>
        /// 
        /// </summary>
        BitBlt,
        //DesktopDuplicationAPI
    }
    /// <summary>
    /// 
    /// </summary>
    public class LdPlayerHelper
    {
        const int Srccopy = 0x00CC0020;

        readonly IntPtr TopWindowHandle;
        readonly IntPtr BindWindowHandle;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TopWindowHandle"></param>
        /// <param name="BindWindowHandle"></param>
        public LdPlayerHelper(IntPtr TopWindowHandle, IntPtr BindWindowHandle)
        {
            this.TopWindowHandle = TopWindowHandle;
            this.BindWindowHandle = BindWindowHandle;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Resize(Size size)
        {
            if (User32.GetWindowRect(TopWindowHandle, out RECT windowRect))
            {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                if (size.Width != width && size.Height != height)
                {
                    return User32.SetWindowPos(
                      TopWindowHandle,
                      IntPtr.Zero,
                      windowRect.left, windowRect.top,
                      size.Width, size.Height, User32.SetWindowPosFlags.SWP_SHOWWINDOW);
                }
                return true;
            }
            return false;
        }

        //https://stackoverflow.com/questions/5069104/fastest-method-of-screen-capturing-on-windows

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Bitmap ScreenShot(ScreenShotType type)
        {
            if (User32.GetWindowRect(TopWindowHandle, out RECT windowRect))// get the size
            {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;
                switch (type)
                {
                    case ScreenShotType.PrintWindow:
                        {
                            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            using Graphics fgx = Graphics.FromImage(bitmap);
                            IntPtr hdc = fgx.GetHdc();
                            bool flag = User32.PrintWindow(TopWindowHandle, hdc, User32.PrintWindowFlags.PW_FULLWINDOW);
                            fgx.ReleaseHdc(hdc);
                            if (flag) return bitmap;
                            else
                            {
                                using (var bm = bitmap) return null;
                            }
                        }

                    case ScreenShotType.BitBlt:
                        {
                            // get te hDC of the target window
                            using var hdcSrc = User32.GetWindowDC(TopWindowHandle);
                            if (hdcSrc.IsInvalid)
                                return null;

                            // create a device context we can copy to
                            using var hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
                            if (hdcDest.IsInvalid)
                                return null;

                            // create a bitmap we can copy it to,
                            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
                            // select the bitmap object
                            IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);

                            // bitblt over
                            bool result = Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Srccopy);

                            try
                            {
                                if (result)
                                {
                                    // restore selection
                                    IntPtr obj = Gdi32.SelectObject(hdcDest, hOld);
                                    //ReleaseDC(TopWindowHandle, hdcSrc);//auto release by "using hdcSrc"
                                    return Image.FromHbitmap(hBitmap);
                                }
                                else return null;
                            }
                            finally
                            {
                                // clean up
                                result = Gdi32.DeleteDC(hdcDest);

                                // free up the Bitmap object
                                result = Gdi32.DeleteObject(hBitmap);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="fixTop"></param>
        public void Tap(Point point, int delay = 50, int fixTop = 34)
        {
            BindWindowHandle.ControlLClick(point.X, point.Y - fixTop, delay);
        }

        const uint MK_LBUTTON = 0x0001;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        /// <param name="step"></param>
        /// <param name="fixTop"></param>
        /// <param name="cancellationToken"></param>
        public void Swipe(Point from, Point to, int time, int step = 10, int fixTop = 34, CancellationToken cancellationToken = default)
        {
            int times = time / step;

            int x_step = (to.X - from.X) / times;
            int y_step = (to.Y - from.Y) / times;

            int x = from.X;
            int y = from.Y;
            try
            {
                User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y - fixTop));
                for (int i = 1; i < times; i++)
                {
                    Task.Delay(step, cancellationToken).Wait();
                    x = from.X + i * x_step;
                    y = from.Y + i * y_step;
                    User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y - fixTop));
                }
                Task.Delay(step, cancellationToken).Wait();
            }
            finally
            {
                User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(to.X, to.Y - fixTop));
                User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(to.X, to.Y - fixTop));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        public void SendText(string text, int delay = 10)
        {
            foreach (char c in text)
            {
                BindWindowHandle.SendKey(c);
                Task.Delay(delay).Wait();
            }
        }

        /// <summary>
        /// for key code, try find at <see href="https://keycode.info/"/>
        /// </summary>
        /// <returns></returns>
        public async Task SendTextAsync(string text, int delay = 10, CancellationToken cancellationToken = default)
        {
            foreach (char c in text)
            {
                BindWindowHandle.SendKey(c);
                await Task.Delay(delay, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendTextUnicode(string text, int delay = 10)
        {
            if (User32.IsWindowUnicode(BindWindowHandle))
            {
                foreach (char c in text)
                {
                    User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_CHAR, new IntPtr(c), IntPtr.Zero);
                    Task.Delay(delay).Wait();
                }
            }
            else throw new NotSupportedException("This process not support unicode");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="NotSupportedException"></exception>
        public async Task SendTextUnicodeAsync(string text, int delay = 10, CancellationToken cancellationToken = default)
        {
            if (User32.IsWindowUnicode(BindWindowHandle))
            {
                foreach (char c in text)
                {
                    User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_CHAR, new IntPtr(c), IntPtr.Zero);
                    await Task.Delay(delay, cancellationToken);
                }
            }
            else throw new NotSupportedException("This process not support unicode");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Key(int key)
        {
            User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_KEYDOWN, new IntPtr(key), IntPtr.Zero);
            User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_CHAR, new IntPtr(key), IntPtr.Zero);
            User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_KEYUP, new IntPtr(key), IntPtr.Zero);
        }
    }
}
