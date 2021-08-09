using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi
{
  public static class Helpers
  {
    public static Bitmap CaptureWindow(IntPtr handle)
    {
      User32.GetWindowRect(handle, out RECT windowRect);// get the size
      int width = windowRect.right - windowRect.left;
      int height = windowRect.bottom - windowRect.top;

      Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      using Graphics fgx = Graphics.FromImage(bitmap);
      IntPtr hdc = fgx.GetHdc();
      User32.PrintWindow(handle, hdc, User32.PrintWindowFlags.PW_FULLWINDOW);
      fgx.ReleaseHdc(hdc);

      return bitmap;
    }
  }
}
