using PInvoke;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Enums;

namespace TqkLibrary.WinApi.HandleControls
{
    /// <summary>
    /// 
    /// </summary>
    public class MemuAndroidEmulatorHelper
    {
        /// <summary>
        /// default 32
        /// </summary>
        public int FixPosClickY { get; } = 32;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// 
        /// </summary>
        public MemuAndroidEmulatorHelper(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle));
            this.Handle = handle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Resize(Size size) => Handle.Resize(size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="captureType"></param>
        /// <returns></returns>
        public Bitmap Capture(CaptureType captureType = CaptureType.PrintWindow) => this.Handle.Capture(captureType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ClickAsync(Point point, CancellationToken cancellationToken = default)
        {
            Handle.MemuControlLClick(new Point(point.X, point.Y - FixPosClickY));
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SendTextAsync(string text, int delay = 10, CancellationToken cancellationToken = default)
            => Handle.SendTextUnicodeAsync(text, delay, cancellationToken);
    }
    static class MemuExtensions
    {
        static IntPtr htclient_wm_mousemove = (IntPtr)0x02000001;
        const uint MK_LBUTTON = 0x0001;
        internal static void MemuControlLClick(this IntPtr windowHandle, Point point)
        {
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_SETCURSOR, windowHandle, htclient_wm_mousemove);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_MOUSEMOVE, IntPtr.Zero, point.ToLParam());
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_MOUSELEAVE, IntPtr.Zero, IntPtr.Zero);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), point.ToLParam());
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(0), point.ToLParam());
        }
    }
}
