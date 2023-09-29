using PInvoke;
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
        public static IEnumerable<IntPtr> GetWindowsOfProcess(this int process_id)
        {
            IReadOnlyList<IntPtr> windows = EnumWindows();
            IReadOnlyList<IntPtr> childWindows = GetChildWindows(IntPtr.Zero);
            foreach (IntPtr hWnd in windows.Concat(childWindows).Distinct())
            {
                User32.GetWindowThreadProcessId(hWnd, out int lpdwProcessId);
                if (lpdwProcessId == process_id)
                    yield return hWnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<IntPtr> EnumWindows()
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                User32.WNDENUMPROC wndEnumProc = EnumWindow;
                User32.EnumWindows(wndEnumProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
        static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
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
        public static IReadOnlyList<IntPtr> GetChildWindows(this IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                User32.WNDENUMPROC wndEnumProc = EnumWindow;
                IntPtr intPtrchildProc = Marshal.GetFunctionPointerForDelegate(wndEnumProc);
                User32.EnumChildWindows(parent, intPtrchildProc, GCHandle.ToIntPtr(listHandle));
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
