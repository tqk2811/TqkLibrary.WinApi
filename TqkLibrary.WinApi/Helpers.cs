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
        public static Bitmap CaptureWindow(this IntPtr handle)
        {
            if (User32.GetWindowRect(handle, out RECT windowRect))// get the size
            {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using Graphics fgx = Graphics.FromImage(bitmap);
                IntPtr hdc = fgx.GetHdc();
                User32.PrintWindow(handle, hdc, User32.PrintWindowFlags.PW_FULLWINDOW);
                fgx.ReleaseHdc(hdc);

                return bitmap;
            }
            return null;
        }

        const uint MK_LBUTTON = 0x0001;
        public static void ControlLClick(this IntPtr windowHandle, int x, int y, int delay = 50)
        {
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y));
            Task.Delay(delay).Wait();
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(MK_LBUTTON), Helpers.CreateLParam(x, y));
        }

        /// <summary>
        /// https://keycode.info/
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="chr"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static void SendKey(this IntPtr windowHandle, char chr)
        {
            bool isShift = false;
            int key = 0;
            if (97 <= chr && chr <= 122)// A-Z
            {
                isShift = true;
                key = chr - ('a' - 'A');
            }
            else if (65 <= chr && chr <= 90)//a-z
            {
                key = chr;
            }
            else if (48 <= chr && chr <= 57)//0-9
            {
                key = chr;
            }
            else
            {
                switch (chr)
                {
                    case '!':
                        {
                            key = '1';
                            isShift = true;
                            break;
                        }
                    case '@':
                        {
                            key = '2';
                            isShift = true;
                            break;
                        }
                    case '#':
                        {
                            key = '3';
                            isShift = true;
                            break;
                        }
                    case '$':
                        {
                            key = '4';
                            isShift = true;
                            break;
                        }
                    case '%':
                        {
                            key = '5';
                            isShift = true;
                            break;
                        }
                    case '^':
                        {
                            key = '6';
                            isShift = true;
                            break;
                        }
                    case '&':
                        {
                            key = '7';
                            isShift = true;
                            break;
                        }
                    case '*':
                        {
                            key = '8';
                            isShift = true;
                            break;
                        }
                    case '(':
                        {
                            key = '9';
                            isShift = true;
                            break;
                        }
                    case ')':
                        {
                            key = '0';
                            isShift = true;
                            break;
                        }
                    case '_':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_MINUS;//-
                            isShift = true;
                            break;
                        }

                    case '=':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_PLUS;
                            break;
                        }
                    case '+':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_PLUS;
                            isShift = true;
                            break;
                        }

                    case '[':
                        {
                            key = (int)219;
                            break;
                        }
                    case '{':
                        {
                            key = (int)219;
                            isShift = true;
                            break;
                        }

                    case ']':
                        {
                            key = (int)221;
                            break;
                        }
                    case '}':
                        {
                            key = (int)221;
                            isShift = true;
                            break;
                        }

                    case '\\':
                        {
                            key = (int)220;
                            break;
                        }
                    case '|':
                        {
                            key = (int)220;
                            isShift = true;
                            break;
                        }

                    case ',':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_COMMA;//,
                            break;
                        }
                    case '<':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_COMMA;//,
                            isShift = true;
                            break;
                        }

                    case '.':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_PERIOD;//.
                            break;
                        }
                    case '>':
                        {
                            key = (int)User32.VirtualKey.VK_OEM_PERIOD;//.
                            isShift = true;
                            break;
                        }

                    case '/':
                        {
                            key = (int)User32.VirtualKey.VK_DIVIDE;//.
                            break;
                        }
                    case '?':
                        {
                            key = (int)User32.VirtualKey.VK_DIVIDE;//.
                            isShift = true;
                            break;
                        }

                    default:
                        throw new NotSupportedException(chr.ToString());
                }
            }

            User32.SendMessage(windowHandle, User32.WindowMessage.WM_KEYDOWN, new IntPtr((uint)key), IntPtr.Zero);
            if (isShift) User32.SendMessage(windowHandle, User32.WindowMessage.WM_KEYDOWN, new IntPtr((uint)User32.VirtualKey.VK_SHIFT), IntPtr.Zero);
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_CHAR, new IntPtr((uint)chr), IntPtr.Zero);
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_KEYUP, new IntPtr((uint)key), IntPtr.Zero);
            if (isShift) User32.SendMessage(windowHandle, User32.WindowMessage.WM_KEYDOWN, new IntPtr((uint)User32.VirtualKey.VK_SHIFT), IntPtr.Zero);
        }


        public static IntPtr CreateLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }
}
