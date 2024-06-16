using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
                if (PInvoke.GetWindowRect((HWND)WindowHandle, out RECT rect))
                {
                    return (Rectangle)rect;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public unsafe string Title
        {
            get
            {
                //window title 
                int maxLength = PInvoke.GetWindowTextLength((HWND)WindowHandle);
                if (maxLength > 0)
                {
                    char[] text = new char[maxLength + 1];
                    int finalLength = 0;
                    fixed (char* textPtr = text)
                    {
                        finalLength = PInvoke.GetWindowText((HWND)WindowHandle, textPtr, maxLength + 1);
                        if (finalLength > 0)
                        {
                            return new string(text, 0, finalLength);
                        }
                    }
                }

                //from control of another process
                //https://stackoverflow.com/a/7740920/5034139
                nint titleSize = SendMessage(PInvoke.WM_GETTEXTLENGTH, 0, 0);
                if (titleSize > 0)
                {
                    char[] text = new char[titleSize + 1];
                    nint finalLength = 0;
                    fixed (char* textPtr = text)
                    {
                        finalLength = SendMessage(PInvoke.WM_GETTEXT, (nuint)text.Length, (IntPtr)textPtr);
                        if(finalLength > 0)
                        {
                            return new string(text, 0, (int)finalLength);
                        }
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public unsafe string ClassName
        {
            get
            {
                char[] text = new char[129];
                int finalLength = 0;
                fixed (char* textPtr = text)
                {
                    finalLength = PInvoke.GetClassName((HWND)WindowHandle, textPtr, text.Length);
                    if (finalLength == 0)
                    {
                        return string.Empty;
                    }
                }
                return new string(text, 0, finalLength);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWindowVisible
        {
            get
            {
                return PInvoke.IsWindowVisible((HWND)WindowHandle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWindowUnicode
        {
            get
            {
                return PInvoke.IsWindowUnicode((HWND)WindowHandle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWindow
        {
            get
            {
                return PInvoke.IsWindow((HWND)WindowHandle);
            }
        }

        /// <summary>
        /// Determines whether the specified window is minimized (iconic).
        /// </summary>
        public bool IsIconic
        {
            get
            {
                return PInvoke.IsIconic((HWND)WindowHandle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WindowHelper? ParentWindow
        {
            get
            {
                IntPtr handle = PInvoke.GetParent((HWND)WindowHandle);
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
                foreach (HWND item in ((HWND)WindowHandle).GetChildWindows())
                {
                    IntPtr handle = PInvoke.GetParent(item);
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
                foreach (HWND item in ((HWND)WindowHandle).GetChildWindows())
                {
                    yield return new WindowHelper(item);
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public unsafe ProcessHelper? Process
        {
            get
            {
                //System.win
                uint ProcessId = 0;
                uint res = PInvoke.GetWindowThreadProcessId((HWND)WindowHandle, &ProcessId);
                if (res != 0)
                {
                    return new ProcessHelper(ProcessId);
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is WindowHelper windowHelper)
            {
                return windowHelper.WindowHandle == WindowHandle;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return WindowHandle.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(WindowHandle)}:{WindowHandle}, {nameof(Title)}: '{Title}', {nameof(ClassName)}: '{ClassName}'";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public nint SendMessage(uint msg, nuint wParam, nint lParam)
        {
            return PInvoke.SendMessage((HWND)WindowHandle, msg, (WPARAM)wParam, (LPARAM)lParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public nint PostMessage(uint msg, nuint wParam, nint lParam)
        {
            return PInvoke.PostMessage((HWND)WindowHandle, msg, (WPARAM)wParam, (LPARAM)lParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool SetWindowText(string text)
        {
            return PInvoke.SetWindowText((HWND)WindowHandle, text);
        }





        /// <summary>
        /// 
        /// </summary>
        public static IReadOnlyList<WindowHelper> AllWindows
        {
            get
            {
                return Extensions.EnumWindows()
                    .Select(x => new WindowHelper(x))
                    .ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WindowHelper? GetFocus()
        {
            HWND hwnd = PInvoke.GetFocus();
            if (hwnd.IsNull) return null;
            return new WindowHelper(hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WindowHelper? GetActiveWindow()
        {
            HWND hwnd = PInvoke.GetActiveWindow();
            if (hwnd.IsNull) return null;
            return new WindowHelper(hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static WindowHelper? WindowFromPoint(Point point)
        {
            HWND hwnd = PInvoke.WindowFromPoint(point);
            if (hwnd.IsNull) return null;
            return new WindowHelper(hwnd);
        }
    }
}
