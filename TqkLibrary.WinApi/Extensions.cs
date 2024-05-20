using System;
using System.Drawing;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool Resize(this IntPtr windowHandle, Size size)
        {
            if (PInvoke.GetWindowRect((HWND)windowHandle, out RECT windowRect))
            {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                if (size.Width != width && size.Height != height)
                {
                    return PInvoke.SetWindowPos(
                      (HWND)windowHandle,
                      default,
                      windowRect.left, windowRect.top,
                      size.Width, size.Height, SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="LoWord"></param>
        /// <param name="HiWord"></param>
        /// <returns></returns>
        public static IntPtr CreateLParam(int LoWord, int HiWord) => (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IntPtr ToLParam(this Point point) => CreateLParam(point.X, point.Y);


    }
}
