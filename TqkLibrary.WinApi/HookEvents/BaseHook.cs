//https://github.com/justcoding121/windows-user-action-hook
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
        internal static FreeLibrarySafeHandle GetUser32Module()
        {
            return PInvoke.GetModuleHandle("user32");
        }



        private UnhookWindowsHookExSafeHandle? _windowsHookExHandle;
        /// <summary>
        /// 
        /// </summary>
        internal UnhookWindowsHookExSafeHandle? WindowsHookExHandle
        {
            get { return _windowsHookExHandle; }
            set
            {
                _windowsHookExHandle = value;
            }
        }


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
        internal virtual LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
        {
            return PInvoke.CallNextHookEx(WindowsHookExHandle, nCode, wParam, lParam);
        }
    }
}