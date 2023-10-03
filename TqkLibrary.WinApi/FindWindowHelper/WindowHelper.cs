using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WinApi.PInvokeAdv.Api;

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
        /// <param name="windowHandle"></param>
        public WindowHelper(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr WindowHandle { get; }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle? Area
        {
            get
            {
                if (User32.GetWindowRect(WindowHandle, out RECT rect))
                {
                    return rect.GetRectangle();
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get { return User32.GetWindowText(WindowHandle); } }

        /// <summary>
        /// 
        /// </summary>
        public WindowHelper ParentWindow
        {
            get
            {
                IntPtr handle = User32.GetParent(WindowHandle);
                if (handle == IntPtr.Zero) return null;
                return new WindowHelper(handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> ChildrensWindow
        {
            get
            {
                foreach (var item in WindowHandle.GetChildWindows())
                {
                    IntPtr handle = User32.GetParent(item);
                    if (handle == WindowHandle)
                    {
                        yield return new WindowHelper(item);
                    }
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> AllChildrensWindow
        {
            get
            {
                foreach (var item in WindowHandle.GetChildWindows())
                {
                    yield return new WindowHelper(item);
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProcessHelper Process
        {
            get
            {
                int res = User32.GetWindowThreadProcessId(WindowHandle, out int lpdwProcessId);
                if (res != 0)
                {
                    return new ProcessHelper(lpdwProcessId);
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is WindowHelper windowHelper)
            {
                return windowHelper.WindowHandle == this.WindowHandle;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.WindowHandle.GetHashCode();
        }
    }
}
