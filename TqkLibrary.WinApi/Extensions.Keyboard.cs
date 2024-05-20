using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Structs;

namespace TqkLibrary.WinApi
{
    public static partial class Extensions
    {
        static readonly IReadOnlyDictionary<char, int> map_chars_shift = new Dictionary<char, int>()
        {
            { '!', '1' },
            { '@', '2' },
            { '#', '3' },
            { '$', '4' },
            { '%', '5' },
            { '^', '6' },
            { '&', '7' },
            { '*', '8' },
            { '(', '9' },
            { ')', '0' },
            { '_', PInvoke.VK_OEM_MINUS },
            { '+', PInvoke.VK_OEM_PLUS },
            { '<', PInvoke.VK_OEM_COMMA },
            { '>', PInvoke.VK_OEM_PERIOD },
            { '{', 219 },
            { '|', 220 },
            { '}', 221 },
            { '?', PInvoke.VK_DIVIDE },
        };
        static readonly IReadOnlyDictionary<char, int> map_chars = new Dictionary<char, int>()
        {
            { '-', PInvoke.VK_OEM_MINUS },
            { '=', PInvoke.VK_OEM_PLUS },
            { ',', PInvoke.VK_OEM_COMMA },
            { '.', PInvoke.VK_OEM_PERIOD },
            { '[', 219 },
            { '\\', 220 },
            { ']', 221 },
            { '/', PInvoke.VK_DIVIDE },
        };


        /// <summary>
        /// https://keycode.info/
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="chr"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static void SendCharAsKey(this IntPtr windowHandle, char chr)
        {
            bool isShift = false;
            bool isCaplock = GetKeyState(PInvoke.VK_CAPITAL).IsToggled;
            int key = 0;

            if (97 <= chr && chr <= 122)// A-Z
            {
                if (!isCaplock) isShift = true;
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
                if (map_chars_shift.ContainsKey(chr))
                {
                    key = map_chars_shift[chr];
                    isShift = true;
                }
                else if (map_chars.ContainsKey(chr))
                {
                    key = map_chars[chr];
                    isShift = false;
                }
                else
                {
                    //switch (chr)
                    //{
                    //    default:
                    throw new NotSupportedException(chr.ToString());
                    //}
                }
            }
            HWND hwnd = (HWND)windowHandle;
            PInvoke.SendMessage(hwnd, PInvoke.WM_KEYDOWN, new WPARAM((uint)key), IntPtr.Zero);
            if (isShift) PInvoke.SendMessage(hwnd, PInvoke.WM_KEYDOWN, new WPARAM(PInvoke.VK_SHIFT), IntPtr.Zero);
            PInvoke.SendMessage(hwnd, PInvoke.WM_CHAR, new WPARAM((uint)chr), IntPtr.Zero);
            PInvoke.SendMessage(hwnd, PInvoke.WM_KEYUP, new WPARAM((uint)key), IntPtr.Zero);
            if (isShift) PInvoke.SendMessage(hwnd, PInvoke.WM_KEYDOWN, new WPARAM(PInvoke.VK_SHIFT), IntPtr.Zero);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualKey"></param>
        /// <returns></returns>
        public static KeyState GetKeyState(int virtualKey) => new KeyState(PInvoke.GetKeyState(virtualKey));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static async Task SendTextUnicodeAsync(this IntPtr handle, string text, int delay = 10, CancellationToken cancellationToken = default)
        {
            if (PInvoke.IsWindowUnicode((HWND)handle))
            {
                foreach (char c in text)
                {
                    PInvoke.SendMessage((HWND)handle, PInvoke.WM_CHAR, new WPARAM(c), IntPtr.Zero);
                    await Task.Delay(delay, cancellationToken);
                }
            }
            else throw new NotSupportedException("This process not support unicode");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void Key(this IntPtr handle, uint key)
        {
            PInvoke.SendMessage((HWND)handle, PInvoke.WM_KEYDOWN, new WPARAM(key), IntPtr.Zero);
            PInvoke.SendMessage((HWND)handle, PInvoke.WM_CHAR, new WPARAM(key), IntPtr.Zero);
            PInvoke.SendMessage((HWND)handle, PInvoke.WM_KEYUP, new WPARAM(key), IntPtr.Zero);
        }
    }
}
