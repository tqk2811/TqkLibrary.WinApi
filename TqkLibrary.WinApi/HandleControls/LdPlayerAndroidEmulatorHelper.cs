using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TqkLibrary.WinApi.Enums;

namespace TqkLibrary.WinApi.HandleControls
{
    /// <summary>
    /// 
    /// </summary>
    public class LdPlayerAndroidEmulatorHelper
    {
        /// <summary>
        /// default 34
        /// </summary>
        public int FixPosClickY { get; set; } = 34;
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
        public Bitmap Capture(CaptureType type) => TopWindowHandle.Capture(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ClickAsync(Point point, int delay = 50, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlLClickAsync(new Point(point.X, point.Y - FixPosClickY), delay, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        public Task Swipe(Point from, Point to, int duration = 500, int step = 10, CancellationToken cancellationToken = default)
            => BindWindowHandle.ControlLSwipeAsync(new Point(from.X, from.Y - FixPosClickY), new Point(to.X, to.Y - FixPosClickY), duration, step, cancellationToken);

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
        public void Key(int key) => BindWindowHandle.Key(key);
    }
}
