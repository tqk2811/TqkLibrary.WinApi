using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TqkLibrary.WinApi
{
  public class LdPlayerHelper
  {
    static readonly object lock_win32 = new object();
    readonly IntPtr TopWindowHandle;
    readonly IntPtr BindWindowHandle;
    readonly Size windowSize;
    public LdPlayerHelper(IntPtr TopWindowHandle, IntPtr BindWindowHandle, Size windowSize)
    {
      this.TopWindowHandle = TopWindowHandle;
      this.BindWindowHandle = BindWindowHandle;
      this.windowSize = windowSize;
    }


    public Bitmap ScreenShot()
    {
      if (User32.GetWindowRect(TopWindowHandle, out RECT windowRect))// get the size
      {
        int width = windowRect.right - windowRect.left;
        int height = windowRect.bottom - windowRect.top;
        if (windowSize.Width != width && windowSize.Height != height)
        {
          User32.SetWindowPos(
            TopWindowHandle,
            IntPtr.Zero,
            windowRect.left, windowRect.top,
            windowSize.Width, windowSize.Height, User32.SetWindowPosFlags.SWP_SHOWWINDOW);
          Task.Delay(100).Wait();
          if (!User32.GetWindowRect(TopWindowHandle, out windowRect)) return null;
          else
          {
            width = windowRect.right - windowRect.left;
            height = windowRect.bottom - windowRect.top;
          }
        }

        Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using Graphics fgx = Graphics.FromImage(bitmap);
        IntPtr hdc = fgx.GetHdc();
        User32.PrintWindow(TopWindowHandle, hdc, User32.PrintWindowFlags.PW_FULLWINDOW);
        fgx.ReleaseHdc(hdc);
        return bitmap;
      }
      return null;
    }

    public void Tap(Point point,int delay = 50, int fixTop = 34)
    {
      BindWindowHandle.ControlLClick(point.X, point.Y - fixTop, delay);
    }

    const uint MK_LBUTTON = 0x0001;
    public void Swipe(Point from, Point to, int time, int step = 10, int fixTop = 34, CancellationToken cancellationToken = default)
    {
      int times = time / step;

      int x_step = (to.X - from.X) / times;
      int y_step = (to.Y - from.Y) / times;

      int x = from.X;
      int y = from.Y;
      try
      {
        User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y - fixTop));
        for (int i = 1; i < times; i++)
        {
          Task.Delay(step, cancellationToken).Wait();
          x = from.X + i * x_step;
          y = from.Y + i * y_step;
          User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y - fixTop));
        }
        Task.Delay(step, cancellationToken).Wait();
      }
      finally
      {
        User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_MOUSEMOVE, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(to.X, to.Y - fixTop));
        User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(to.X, to.Y - fixTop));
      }
    }


    public void SendText(string text,int delay = 10)
    {
      foreach(char c in text)
      {
        BindWindowHandle.SendKey(c);
        Task.Delay(delay).Wait();
      }
    }

    public void Key(int key)
    {
      User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_KEYDOWN, new IntPtr(key), IntPtr.Zero);
      User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_CHAR, new IntPtr(key), IntPtr.Zero);
      User32.SendMessage(BindWindowHandle, User32.WindowMessage.WM_KEYUP, new IntPtr(key), IntPtr.Zero);
    }
  }
}
