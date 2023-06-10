using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Enums;

namespace TqkLibrary.WinApi
{
	/// <summary>
	/// 
	/// </summary>
	public static partial class Extensions
	{
		const int Srccopy = 0x00CC0020;

        //https://stackoverflow.com/questions/5069104/fastest-method-of-screen-capturing-on-windows
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPtr">Window handle</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bitmap Capture(this IntPtr intPtr, CaptureType type = CaptureType.PrintWindow)
		{
			if (User32.GetWindowRect(intPtr, out RECT windowRect))// get the size
			{
				int width = windowRect.right - windowRect.left;
				int height = windowRect.bottom - windowRect.top;
				switch (type)
				{
					case CaptureType.PrintWindow:
						{
							Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							using Graphics fgx = Graphics.FromImage(bitmap);
							IntPtr hdc = fgx.GetHdc();
							bool flag = User32.PrintWindow(intPtr, hdc, User32.PrintWindowFlags.PW_FULLWINDOW);
							fgx.ReleaseHdc(hdc);
							if (flag) return bitmap;
							else
							{
								using (var bm = bitmap) return null;
							}
						}

					case CaptureType.BitBlt:
						{
							// get te hDC of the target window
							using var hdcSrc = User32.GetWindowDC(intPtr);
							if (hdcSrc.IsInvalid)
								return null;

							// create a device context we can copy to
							using var hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
							if (hdcDest.IsInvalid)
								return null;

							// create a bitmap we can copy it to,
							IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
							// select the bitmap object
							IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);

							// bitblt over
							bool result = Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Srccopy);

							try
							{
								if (result)
								{
									// restore selection
									IntPtr obj = Gdi32.SelectObject(hdcDest, hOld);
									//ReleaseDC(TopWindowHandle, hdcSrc);//auto release by "using hdcSrc"
									return Image.FromHbitmap(hBitmap);
								}
								else return null;
							}
							finally
							{
								// clean up
								result = Gdi32.DeleteDC(hdcDest);

								// free up the Bitmap object
								result = Gdi32.DeleteObject(hBitmap);
							}
						}

						//case ScreenShotType.DesktopDuplicationAPI:
						//    {

						//        break;
						//    }
				}
			}
			return null;
		}
	}
}
