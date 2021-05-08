using PInvoke;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
  public static class WindowApiHelper
  {
    public static IntPtr StructureToPtr<T>(T _struct) where T : struct
    {
      int msgSize = Marshal.SizeOf(_struct);
      IntPtr ptr = Marshal.AllocCoTaskMem(msgSize);
      Marshal.StructureToPtr(_struct, ptr, true);
      return ptr;
    }

    public static T IntPtrToStructObject<T>(IntPtr intptr) where T : struct
    {
      return (T)Marshal.PtrToStructure(intptr, typeof(T));
    }

    public static Rectangle GetRectangle(this RECT r) => new Rectangle(r.left, r.top, r.right, r.bottom);
  }
}