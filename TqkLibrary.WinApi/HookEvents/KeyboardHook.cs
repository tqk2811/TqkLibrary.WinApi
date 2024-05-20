using System;
using System.Runtime.InteropServices;
using System.Threading;

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
            public nuint Message { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public int VirtualKeyCode { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public bool IsKeyDown
            {
                get { return Message == PInvoke.WM_KEYDOWN || Message == PInvoke.WM_SYSKEYDOWN; }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RawKeyboardEventArgs>? KeyboardAction;

        private HOOKPROC _hookCallback;
        /// <summary>
        /// 
        /// </summary>
        public KeyboardHook()
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
                    WINDOWS_HOOK_ID.WH_KEYBOARD_LL,
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
            if (KeyboardAction is not null &&
                nCode >= 0 &&
                (
                  wParam == PInvoke.WM_KEYDOWN ||
                  wParam == PInvoke.WM_KEYUP ||
                  wParam == PInvoke.WM_SYSKEYDOWN ||
                  wParam == PInvoke.WM_SYSKEYUP
                )
            )
            {
                ThreadPool.QueueUserWorkItem((o) => KeyboardAction?.Invoke(this, new RawKeyboardEventArgs()
                {
                    Message = wParam,
                    VirtualKeyCode = Marshal.ReadInt32(lParam),
                }));
            }
            return base.HookCallback(nCode, wParam, lParam);
        }
    }
}