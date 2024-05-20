using System;
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
            public nuint Message { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public Point Point { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public uint MouseData { get; internal set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RawMouseEventArgs>? MouseAction;



        private HOOKPROC _hookCallback;
        /// <summary>
        /// 
        /// </summary>
        public MouseHook()
        {
            _hookCallback = HookCallback;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetupHook()
        {
            if (WindowsHookExHandle is null)
            {
                using FreeLibrarySafeHandle freeLibrarySafeHandle = BaseHook.GetUser32Module();
                WindowsHookExHandle = PInvoke.SetWindowsHookEx(
                    WINDOWS_HOOK_ID.WH_MOUSE_LL,
                    _hookCallback,
                    freeLibrarySafeHandle,
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
        internal override LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
        {
            if (MouseAction is not null && nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                ThreadPool.QueueUserWorkItem((o) => MouseAction?.Invoke(this, new RawMouseEventArgs()
                {
                    Message = wParam,
                    Point = new Point(hookStruct.pt.X, hookStruct.pt.Y),
                    MouseData = hookStruct.mouseData,
                }));
            }
            return base.HookCallback(nCode, wParam, lParam);
        }
    }
}