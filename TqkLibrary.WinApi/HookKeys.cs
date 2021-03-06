using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keycode"></param>
    /// <param name="isDown"></param>
    public delegate void HookCallBack(int keycode, bool isDown);
    /// <summary>
    /// 
    /// </summary>
    public class HookKeys
    {
        /// <summary>
        /// 
        /// </summary>
        public bool HookAll { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public List<int> KeyCode { get; } = new List<int>();

        /// <summary>
        /// 
        /// </summary>

        public event HookCallBack Callback;

        private User32.WindowsHookDelegate windowsHook;
        private User32.SafeHookHandle WindowsHookExHandle;
        private IntPtr handle;
        /// <summary>
        /// 
        /// </summary>
        public HookKeys()
        {
            windowsHook = HookCallback;//make that delegate not release by GC
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetupHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                IntPtr ModuleHandle = Kernel32.GetModuleHandle(curModule.ModuleName);
                if (ModuleHandle != IntPtr.Zero)
                {
                    WindowsHookExHandle = User32.SetWindowsHookEx(User32.WindowsHookType.WH_KEYBOARD_LL, windowsHook, ModuleHandle, 0);
                    handle = WindowsHookExHandle.DangerousGetHandle();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void UnHook()
        {
            if (WindowsHookExHandle != null &&
               !WindowsHookExHandle.IsClosed &&
               !WindowsHookExHandle.IsInvalid)
                WindowsHookExHandle.Dispose();

            WindowsHookExHandle = null;
        }

        private int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (
              wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN ||
              wParam == (IntPtr)User32.WindowMessage.WM_KEYUP ||
              wParam == (IntPtr)User32.WindowMessage.WM_SYSKEYDOWN))
            {
                bool keyDown = wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN || wParam == (IntPtr)User32.WindowMessage.WM_SYSKEYDOWN;
                int vkCode = Marshal.ReadInt32(lParam);
                if (HookAll) ThreadPool.QueueUserWorkItem((o) => Callback?.Invoke(vkCode, keyDown));
                else if (KeyCode.Any(x => x == vkCode)) ThreadPool.QueueUserWorkItem((o) => Callback?.Invoke(vkCode, keyDown));
            }
            return User32.CallNextHookEx(handle, nCode, wParam, lParam);
        }
    }
}