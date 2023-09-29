using PInvoke;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindowApiHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_struct"></param>
        /// <returns></returns>
        public static IntPtr StructureToPtr<T>(T _struct) where T : struct
        {
            int msgSize = Marshal.SizeOf(_struct);
            IntPtr ptr = Marshal.AllocCoTaskMem(msgSize);
            Marshal.StructureToPtr(_struct, ptr, true);
            return ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="intptr"></param>
        /// <returns></returns>
        public static T IntPtrToStructObject<T>(IntPtr intptr) where T : struct
        {
            return (T)Marshal.PtrToStructure(intptr, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rectangle GetRectangle(this RECT r) => new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
    }
}