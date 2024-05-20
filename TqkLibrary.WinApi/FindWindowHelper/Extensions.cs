using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process_id"></param>
        /// <returns></returns>
        internal static unsafe IEnumerable<HWND> GetWindowsOfProcess(this uint process_id)
        {
            List<HWND> result = new List<HWND>();
            IReadOnlyList<HWND> windows = EnumWindows();
            IReadOnlyList<HWND> childWindows = GetChildWindows((HWND)IntPtr.Zero);
            foreach (HWND hWnd in windows.Concat(childWindows).Distinct())
            {
                uint ProcessId = 0;
                PInvoke.GetWindowThreadProcessId(hWnd, &ProcessId);
                if (ProcessId == process_id)
                    result.Add(hWnd);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static IReadOnlyList<HWND> EnumWindows()
        {
            List<HWND> result = new List<HWND>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                WNDENUMPROC wndEnumProc = EnumWindow;
                PInvoke.EnumWindows(wndEnumProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
        static BOOL EnumWindow(HWND handle, LPARAM pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<HWND>? list = gch.Target as List<HWND>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static IReadOnlyList<HWND> GetChildWindows(this HWND parent)
        {
            List<HWND> result = new List<HWND>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                WNDENUMPROC wndEnumProc = EnumWindow;
                PInvoke.EnumChildWindows(parent, wndEnumProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
    }
}
