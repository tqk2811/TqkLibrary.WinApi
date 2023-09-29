using PInvoke;
using System.Runtime.InteropServices;
using System.Threading;
using System;

namespace TqkLibrary.WinApi.HookEvents
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyboardHook : BaseHook
    {
        /// <summary>
        /// 
        /// </summary>
        public class RawKeyboardEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            public User32.WindowMessage Message { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public int VirtualKeyCode { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public User32.VirtualKey VirtualKey { get { return (User32.VirtualKey)VirtualKeyCode; } }

            /// <summary>
            /// 
            /// </summary>
            public bool IsKeyDown
            {
                get { return Message == User32.WindowMessage.WM_KEYDOWN || Message == User32.WindowMessage.WM_SYSKEYDOWN; }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RawKeyboardEventArgs> KeyboardAction;

        private User32.WindowsHookDelegate _hookCallback;
        /// <summary>
        /// 
        /// </summary>
        public KeyboardHook()
        {
            this._hookCallback = HookCallback;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetupHook()
        {
            if (WindowsHookExHandle is null)
            {
                WindowsHookExHandle = User32.SetWindowsHookEx(
                    User32.WindowsHookType.WH_KEYBOARD_LL,
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
            if (KeyboardAction is not null &&
                nCode >= 0 &&
                (
                  wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN ||
                  wParam == (IntPtr)User32.WindowMessage.WM_KEYUP ||
                  wParam == (IntPtr)User32.WindowMessage.WM_SYSKEYDOWN ||
                  wParam == (IntPtr)User32.WindowMessage.WM_SYSKEYUP
                )
            )
            {
                ThreadPool.QueueUserWorkItem((o) => KeyboardAction?.Invoke(this, new RawKeyboardEventArgs()
                {
                    Message = (User32.WindowMessage)wParam,
                    VirtualKeyCode = Marshal.ReadInt32(lParam),
                }));
            }
            return base.HookCallback(nCode, wParam, lParam);
        }
    }
}