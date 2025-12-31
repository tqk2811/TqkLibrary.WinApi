using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Enums;

namespace TqkLibrary.WinApi.Helpers.AndroidEmulatorHelpers
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
            Handle = handle;
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
        public Bitmap? Capture(CaptureType captureType = CaptureType.PrintWindow) => Handle.Capture(captureType);

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
            PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_SETCURSOR, new WPARAM((nuint)(nint)windowHandle), htclient_wm_mousemove);
            PInvoke.PostMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, default, point.ToLParam());
            PInvoke.PostMessage((HWND)windowHandle, PInvoke.WM_MOUSELEAVE, default, default);
            PInvoke.PostMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), point.ToLParam());
            PInvoke.PostMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, default, point.ToLParam());
        }
    }
}
