using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), point.ToLParam());
            await Task.Delay(delay, cancellationToken);
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(MK_LBUTTON), point.ToLParam());
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
                User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), CreateLParam(x, y));
                for (int i = 1; i < times; i++)
                {
                    await Task.Delay(step, cancellationToken);
                    x = from.X + i * x_step;
                    y = from.Y + i * y_step;
                    User32.SendMessage(windowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), CreateLParam(x, y));
                }
                await Task.Delay(step, cancellationToken);
            }
            finally
            {
                User32.SendMessage(windowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), CreateLParam(to.X, to.Y));
                User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(MK_LBUTTON), CreateLParam(to.X, to.Y));
            }
        }
    }
}
