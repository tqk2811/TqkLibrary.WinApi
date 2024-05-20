using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi
{
    public static partial class Extensions
    {
        const uint MK_LBUTTON = 0x0001;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ControlLClickAsync(this IntPtr windowHandle, Point point, int delay = 50, CancellationToken cancellationToken = default)
        {
            PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), point.ToLParam());
            await Task.Delay(delay, cancellationToken);
            PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, new WPARAM(MK_LBUTTON), point.ToLParam());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ControlLSwipeAsync(this IntPtr windowHandle,
            Point from, Point to, int duration = 500, int step = 10, CancellationToken cancellationToken = default)
        {
            int times = duration / step;

            int x_step = (to.X - from.X) / times;
            int y_step = (to.Y - from.Y) / times;

            int x = from.X;
            int y = from.Y;
            try
            {
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), CreateLParam(x, y));
                for (int i = 1; i < times; i++)
                {
                    await Task.Delay(step, cancellationToken);
                    x = from.X + i * x_step;
                    y = from.Y + i * y_step;
                    PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(x, y));
                }
                await Task.Delay(step, cancellationToken);
            }
            finally
            {
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(to.X, to.Y));
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, new WPARAM(MK_LBUTTON), CreateLParam(to.X, to.Y));
            }
        }
    }
}
