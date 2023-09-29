using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class WindowHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public IntPtr WindowHandle { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get { return User32.GetWindowText(WindowHandle); } }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public WindowHelper(IntPtr handle)
        {
            WindowHandle = handle;
        }
    }
}
