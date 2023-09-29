//https://github.com/justcoding121/windows-user-action-hook
using PInvoke;
using System;

namespace TqkLibrary.WinApi.HookEvents
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseHook : IHook
    {
        /// <summary>
        /// 
        /// </summary>
        public static IntPtr User32Module { get { return Kernel32.GetModuleHandle("user32"); } }

        private User32.SafeHookHandle _windowsHookExHandle;
        /// <summary>
        /// 
        /// </summary>
        protected User32.SafeHookHandle WindowsHookExHandle
        {
            get { return _windowsHookExHandle; }
            set
            {
                _windowsHookExHandle = value;
                if (value is not null)
                {
                    _hookHandle = value.DangerousGetHandle();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected IntPtr _hookHandle = IntPtr.Zero;

        /// <summary>
        /// 
        /// </summary>
        ~BaseHook()
        {
            UnHook();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            UnHook();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract void SetupHook();
        /// <summary>
        /// 
        /// </summary>
        public void UnHook()
        {
            if (_windowsHookExHandle is not null &&
               !_windowsHookExHandle.IsClosed &&
               !_windowsHookExHandle.IsInvalid)
                _windowsHookExHandle.Dispose();
            _windowsHookExHandle = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected virtual int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return User32.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }
    }
}