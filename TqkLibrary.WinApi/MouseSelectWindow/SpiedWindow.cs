using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TqkLibrary.WinApi.PInvokeAdv.Api;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public class SpiedWindow : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public SpiedWindow(IntPtr handle)
        {
            Handle = handle;
            User32.GetWindowRect(Handle, out RECT rect);
            Area = rect.GetRectangle();
            SetCaption();
        }
        /// <summary>
        /// 
        /// </summary>
        public IntPtr Handle { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Rectangle Area { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Caption { get; private set; }

        private void SetCaption()
        {
            int len = User32.GetWindowTextLength(Handle);
            var str = new StringBuilder(len + 1);
            User32.GetWindowText(Handle, str.ToString().ToCharArray(), len + 1);
            Caption = str.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SpiedWindow GetParentWindow()
        {
            return new SpiedWindow(User32.GetParent(Handle));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentHandle"></param>
        public void SeParentwindow(IntPtr parentHandle)
        {
            User32.SetParent(Handle, parentHandle);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpiedWindow> GetChildren()
        {
            var children = new List<SpiedWindow>();
            User32.EnumChildWindows(Handle, (hWnd, lp) =>
            {
                children.Add(new SpiedWindow(hWnd));
                return true;
            }, IntPtr.Zero);
            return children;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Caption;
        }
    }
}