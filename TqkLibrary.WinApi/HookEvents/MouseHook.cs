using PInvoke;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace TqkLibrary.WinApi.HookEvents
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseHook : BaseHook
    {
        /// <summary>
        /// 
        /// </summary>
        public class RawMouseEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            public User32.WindowMessage Message { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public Point Point { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public uint MouseData { get; internal set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public readonly int x;
            public readonly int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            internal POINT pt;
            internal readonly uint mouseData;
            internal readonly uint flags;
            internal readonly uint time;
            internal readonly IntPtr dwExtraInfo;
        }


        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RawMouseEventArgs> MouseAction;

        private User32.WindowsHookDelegate _hookCallback;
        /// <summary>
        /// 
        /// </summary>
        public MouseHook()
        {
            this._hookCallback = this.HookCallback;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetupHook()
        {
            if (WindowsHookExHandle is null)
            {
                WindowsHookExHandle = User32.SetWindowsHookEx(
                    User32.WindowsHookType.WH_MOUSE_LL,
                    _hookCallback,
                    BaseHook.User32Module,
                    0
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected override int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (MouseAction is not null && nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                ThreadPool.QueueUserWorkItem((o) => MouseAction?.Invoke(this, new RawMouseEventArgs()
                {
                    Message = (User32.WindowMessage)wParam,
                    Point = new Point(hookStruct.pt.x, hookStruct.pt.y),
                    MouseData = hookStruct.mouseData,
                }));
            }
            return base.HookCallback(nCode, wParam, lParam);
        }
    }
}