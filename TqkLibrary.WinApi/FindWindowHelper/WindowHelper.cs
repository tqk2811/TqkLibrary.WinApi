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
                        if (finalLength > 0)
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
        /// Examines the Z order of the child windows associated with the specified parent window and retrieves a handle to the child window at the top of the Z order.
        /// </summary>
        public WindowHelper? GetTopWindow
        {
            get
            {
                IntPtr handle = PInvoke.GetTopWindow((HWND)WindowHandle);
                if (handle == IntPtr.Zero) return null;
                return new WindowHelper(handle);
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
        public IEnumerable<WindowHelper> ChildrenWindows
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
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> AllChildrenWindows
        {
            get
            {
                foreach (HWND item in ((HWND)WindowHandle).GetChildWindows())
                {
                    yield return new WindowHelper(item);
                }
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
        public enum GetAncestorFlags : uint
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the <see cref="ParentWindow"/> function.
            /// </summary>
            GA_PARENT = 1U,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GA_ROOT = 2U,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by <see cref="ParentWindow"/>.
            /// </summary>
            GA_ROOTOWNER = 3U,
        }
        /// <summary>
        /// Retrieves the handle to the ancestor of the specified window.
        /// </summary>
        /// <param name="getAncestorFlags">The ancestor to be retrieved. This parameter can be one of the following values.</param>
        /// <returns>The return value is the handle to the ancestor window.</returns>
        public WindowHelper? GetAncestor(GetAncestorFlags getAncestorFlags)
        {
            IntPtr handle = PInvoke.GetAncestor((HWND)WindowHandle, (GET_ANCESTOR_FLAGS)getAncestorFlags);
            if (handle == IntPtr.Zero) return null;
            return new WindowHelper(handle);
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum SHOW_WINDOW_CMD
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="nCmdShow">Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.</param>
        /// <returns></returns>
        public bool ShowWindow(SHOW_WINDOW_CMD nCmdShow)
        {
            return PInvoke.ShowWindow((HWND)WindowHandle, (Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD)nCmdShow);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAltTabWindow => _CheckIsAltTabWindow(GetShellWindow());
        unsafe bool _CheckIsAltTabWindow(WindowHelper shellWindow)
        {
            if (this == shellWindow)
                return false;
            if (!this.IsWindowVisible)
                return false;
            if (this.GetAncestor(GetAncestorFlags.GA_ROOT) != this)
                return false;

            int style = PInvoke.GetWindowLong((HWND)this.WindowHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            if (!((style & WS_DISABLED) != WS_DISABLED))
                return false;
            if ((style & WS_EX_TOOLWINDOW) != 0)
                return false;
            if ((style & WS_EX_APPWINDOW) != WS_EX_APPWINDOW)
                return false;

            UInt32 cloaked = 0;
            HRESULT hr = PInvoke.DwmGetWindowAttribute(
                (HWND)this.WindowHandle,
                Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED,
                &cloaked,
                sizeof(UInt32)
                );
            if (hr.Succeeded && cloaked == PInvoke.DWM_CLOAKED_SHELL)
                return false;

            if (string.IsNullOrWhiteSpace(this.Title))
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(WindowHelper? a, WindowHelper? b)
        {
            if (a is null && b is null)
                return true;
            if (ReferenceEquals(a, b))
                return true;

            if (a is not null && b is not null)
            {
                return a.WindowHandle == b.WindowHandle;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(WindowHelper? a, WindowHelper b)
        {
            return !(a == b);
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
        public static WindowHelper? GetFocusWindow()
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
        public static WindowHelper? GetWindowFromPoint(Point point)
        {
            HWND hwnd = PInvoke.WindowFromPoint(point);
            if (hwnd.IsNull) return null;
            return new WindowHelper(hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        public static WindowHelper GetShellWindow() => new WindowHelper(PInvoke.GetShellWindow());



        const int WS_DISABLED = 0x08000000;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_APPWINDOW = 0x00040000;
        /// <summary>
        /// 
        /// </summary>
        public static unsafe IEnumerable<WindowHelper> AllAltTabWindows
        {
            get
            {
                WindowHelper shellWindow = GetShellWindow();
                return AllWindows
                    .Where(x => x._CheckIsAltTabWindow(shellWindow));
            }
        }
    }
}
