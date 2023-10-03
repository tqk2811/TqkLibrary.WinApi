using PInvoke;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using TqkLibrary.WinApi.PInvokeAdv.Api;

namespace TqkLibrary.WinApi.IPC
{
    /// <summary>
    /// 
    /// </summary>
    public static class COPYDATA
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static unsafe Win32ErrorCode EnableWM_CopyData(IntPtr windowHandle)
        {
            User32.CHANGEFILTERSTRUCT? changeFilter = new User32.CHANGEFILTERSTRUCT()
            {
                cbSize = (uint)sizeof(User32.CHANGEFILTERSTRUCT),
                ExtStatus = 0
            };

            Kernel32.SetLastError((uint)Win32ErrorCode.ERROR_SUCCESS);

            User32.ChangeWindowMessageFilterEx(
                windowHandle,
                (uint)User32.WindowMessage.WM_COPYDATA,
                (uint)ChangeWindowMessageFilterExAction.Allow,
                ref changeFilter);

            return Kernel32.GetLastError();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="hwndSourceHook"></param>
        public static void Init_WndProc(System.Windows.Window window, HwndSourceHook hwndSourceHook)
        {
            HwndSource source = PresentationSource.FromVisual(window) as HwndSource;
            source.AddHook(hwndSourceHook);
        }
    }
}