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
    public class LdPlayerAndroidEmulatorHelper
    {
        /// <summary>
        /// default 34. Chỉ dùng khi <see cref="SourceSize"/> chưa set (fallback cũ).
        /// </summary>
        public int FixPosClickY { get; set; } = 34;

        /// <summary>
        /// Kích thước ảnh capture mà toạ độ (point/from/to) truyền vào đang dùng (gồm cả title bar).
        /// Set giá trị này để map toạ độ -> client BindWindow theo tỉ lệ (độc lập DPI/scale/awareness).
        /// Để trống (Empty) => dùng <see cref="FixPosClickY"/> như cũ.
        /// </summary>
        public Size SourceSize { get; set; } = Size.Empty;
        const int Srccopy = 0x00CC0020;

        readonly IntPtr TopWindowHandle;
        readonly IntPtr BindWindowHandle;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TopWindowHandle"></param>
        /// <param name="BindWindowHandle"></param>
        public LdPlayerAndroidEmulatorHelper(IntPtr TopWindowHandle, IntPtr BindWindowHandle)
        {
            this.TopWindowHandle = TopWindowHandle;
            this.BindWindowHandle = BindWindowHandle;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Resize(Size size) => TopWindowHandle.Resize(size);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Bitmap? Capture(CaptureType type) => TopWindowHandle.Capture(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ClickAsync(Point point, int delay = 50, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlLClickAsync(_MapToBindClient(point), delay, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        public Task SwipeAsync(Point from, Point to, int duration = 500, int step = 10, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlLSwipeAsync(_MapToBindClient(from), _MapToBindClient(to), duration, step, cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        public Task TouchAsync(Point point, int delay = 50, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlTouchAsync(_MapToBindClient(point), delay, cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        public Task TouchSwipeAsync(Point from, Point to, int duration = 500, int step = 10, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlTouchSwipeAsync(_MapToBindClient(from), _MapToBindClient(to), duration, step, cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        public void SendText(string text, int delay = 10)
        {
            foreach (char c in text)
            {
                BindWindowHandle.SendCharAsKey(c);
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
                BindWindowHandle.SendCharAsKey(c);
                await Task.Delay(delay, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Task SendTextUnicodeAsync(string text, int delay = 10, CancellationToken cancellationToken = default)
            => BindWindowHandle.SendTextUnicodeAsync(text, delay, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Key(uint key) => BindWindowHandle.Key(key);

        /// <summary>
        /// Map toạ độ từ hệ ảnh capture (<see cref="SourceSize"/>, gồm title bar) sang client của BindWindow.
        /// Tỉ lệ tính từ client rect thực của BindWindow đọc tại chỗ -> đúng ở mọi DPI/scale.
        /// </summary>
        Point _MapToBindClient(Point point)
        {
            // chưa set SourceSize -> fallback hành vi cũ
            if (SourceSize.Width <= 0 || SourceSize.Height <= 0)
                return new Point(point.X, point.Y - FixPosClickY);

            if (!PInvoke.GetClientRect((HWND)BindWindowHandle, out RECT rc))
                return new Point(point.X, point.Y - FixPosClickY);

            int bindW = rc.Width;
            int bindH = rc.Height;
            if (bindW <= 0 || bindH <= 0)
                return new Point(point.X, point.Y - FixPosClickY);

            double ratio = bindW / (double)SourceSize.Width;
            // title bar (trong hệ SourceSize) = phần dư chiều cao so với vùng render (cùng tỉ lệ với client BindWindow)
            double titleBar = SourceSize.Height - SourceSize.Width * (bindH / (double)bindW);

            int x = (int)Math.Round(point.X * ratio);
            int y = (int)Math.Round((point.Y - titleBar) * ratio);
            return new Point(x, y);
        }
    }
}
