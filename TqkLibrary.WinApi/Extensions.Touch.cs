using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi
{
    public static partial class Extensions
    {
        public const uint TOUCHEVENTF_MOVE = 0x0001;
        public const uint TOUCHEVENTF_DOWN = 0x0002;
        public const uint TOUCHEVENTF_UP = 0x0004;
        public const uint TOUCHEVENTF_INRANGE = 0x0008;
        public const uint TOUCHEVENTF_PRIMARY = 0x0010;
        public const uint TOUCHINPUTMASKF_CONTACTAREA = 0x0004;

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int x;
            public int y;
            public IntPtr hSource;
            public uint dwID;
            public uint dwFlags;
            public uint dwMask;
            public uint dwTime;
            public UIntPtr dwExtraInfo;
            public uint cxContact;
            public uint cyContact;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="clientPoint"></param>
        /// <param name="flags">TOUCHEVENTF_*</param>
        /// <param name="id"></param>
        public static void SendWmTouch(IntPtr windowHandle, System.Drawing.Point clientPoint, uint flags, uint id = 0)
        {
            System.Drawing.Point screenPoint = clientPoint;
            ClientToScreen(windowHandle, ref screenPoint);

            var ti = new TOUCHINPUT
            {
                x = screenPoint.X * 100,
                y = screenPoint.Y * 100,
                hSource = IntPtr.Zero,
                dwID = id,
                dwFlags = flags,
                dwMask = TOUCHINPUTMASKF_CONTACTAREA,
                dwTime = 0,
                dwExtraInfo = UIntPtr.Zero,
                cxContact = 200,
                cyContact = 200,
            };

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TOUCHINPUT>());
            try
            {
                Marshal.StructureToPtr(ti, ptr, false);
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_TOUCH, new WPARAM(1), ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Send WM_TOUCH down then up at <paramref name="point"/> (client coordinates)
        /// </summary>
        public static async Task ControlTouchAsync(this IntPtr windowHandle, System.Drawing.Point point, int delay = 50, CancellationToken cancellationToken = default)
        {
            SendWmTouch(windowHandle, point, TOUCHEVENTF_DOWN | TOUCHEVENTF_PRIMARY | TOUCHEVENTF_INRANGE);
            await Task.Delay(delay, cancellationToken);
            SendWmTouch(windowHandle, point, TOUCHEVENTF_UP | TOUCHEVENTF_PRIMARY);
        }

        /// <summary>
        /// Send WM_TOUCH swipe from <paramref name="from"/> to <paramref name="to"/> (client coordinates)
        /// </summary>
        public static async Task ControlTouchSwipeAsync(this IntPtr windowHandle,
            System.Drawing.Point from, System.Drawing.Point to,
            int duration = 500, int step = 10, CancellationToken cancellationToken = default)
        {
            int times = duration / step;
            int x_step = (to.X - from.X) / times;
            int y_step = (to.Y - from.Y) / times;

            try
            {
                SendWmTouch(windowHandle, from, TOUCHEVENTF_DOWN | TOUCHEVENTF_PRIMARY | TOUCHEVENTF_INRANGE);
                for (int i = 1; i < times; i++)
                {
                    await Task.Delay(step, cancellationToken);
                    System.Drawing.Point p = new System.Drawing.Point(from.X + i * x_step, from.Y + i * y_step);
                    SendWmTouch(windowHandle, p, TOUCHEVENTF_MOVE | TOUCHEVENTF_PRIMARY | TOUCHEVENTF_INRANGE);
                }
                await Task.Delay(step, cancellationToken);
            }
            finally
            {
                SendWmTouch(windowHandle, to, TOUCHEVENTF_MOVE | TOUCHEVENTF_PRIMARY | TOUCHEVENTF_INRANGE);
                SendWmTouch(windowHandle, to, TOUCHEVENTF_UP | TOUCHEVENTF_PRIMARY);
            }
        }
    }
}
