using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Enums;
using TqkLibrary.WinApi.Helpers;
using TqkLibrary.WinApi.WmiHelpers;
using TqkLibrary.WinApi.Helpers.AndroidEmulatorHelpers;
using TqkLibrary.AdbDotNet.LdPlayers;
using System.Drawing;

namespace TqkLibrary.WinApi.ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var list = WindowHelper.AllAltTabWindows.ToList();
            //var item = list.First();
            //list[0].Area

#if DEBUG
            //foreach (Win32_PnPEntity win32_PnPEntity in BaseWmiDataQueryHelper
            //    .CreateQuery<Win32_PnPEntity>()
            //    //.Include(x => x.Caption, x => x.Name, x => x.PNPClass)
            //    .Where(x => x.PNPClass == "Image" || x.PNPClass == "Camera")
            //    .Query().Where(x => x.PNPClass == "Image" || x.PNPClass == "Camera"))
            //{

            //}
#endif
            //foreach (Win32_PnPEntity win32_PnPEntity in Win32_PnPEntity.Query())
            //{

            //}

            //foreach (ProcessHelper processHelper in Process.GetProcesses().Select(x => new ProcessHelper(x.Id)))
            //{
            //    Win32_Process? win32_Process = processHelper.Query_Win32_Process();
            //}


            //ProcessHelper rootProcessHelper = new ProcessHelper((uint)16760);
            //List<ProcessHelper> processHelpers = new List<ProcessHelper>();
            //processHelpers.Add(rootProcessHelper);
            //processHelpers.AddRange(rootProcessHelper.ChildrensProcess);

            //foreach (ProcessHelper processHelper in processHelpers)
            //{
            //    foreach (WindowHelper windowHelper in processHelper.AllWindows)
            //    {
            //        if (windowHelper.Title?.Contains("Open", StringComparison.OrdinalIgnoreCase) == true)
            //        {
            //            WindowHelper? ComboBoxEx32 = windowHelper.ChildrensWindow.FirstOrDefault(x => x.ClassName.Equals("ComboBoxEx32"));
            //            WindowHelper? ComboBox = ComboBoxEx32?.ChildrensWindow?.FirstOrDefault(x => x.ClassName.Equals("ComboBox"));
            //            WindowHelper? Edit = ComboBox?.ChildrensWindow?.FirstOrDefault(x => x.ClassName.Equals("Edit"));
            //            WindowHelper? openButton = windowHelper.ChildrensWindow.FirstOrDefault(x => x.ClassName.Equals("Button") && x.Title.Equals("&Open"));
            //            if (Edit is not null && openButton is not null)
            //            {
            //                await Edit.WindowHandle.SendTextUnicodeAsync("C:\\Users\\tqk2811\\Pictures\\Otaku\\anemoi\\character_2.png");
            //                uint BM_CLICK = 245U;
            //                openButton.SendMessage(BM_CLICK, 0, 0);
            //                return;
            //            }
            //        }
            //    }
            //}
            //ProcessHelper.Win32_Process? win32_Process = rootProcessHelper.Query_Win32_Process();








            LdPlayer.LdConsolePath = @"D:\LDPlayer\LDPlayer9\ldconsole.exe";
            var list2 = await LdPlayer.List2Async();
            var item = list2.First(x => x.AndroidStarted);

            WindowHelper topHelper = new WindowHelper(item.TopWindowHandle);
            while (true)
            {
                // toạ độ tính theo CLIENT rect của BindWindow (cửa sổ ta gửi message tới),
                // đọc lại mỗi vòng để phản ánh khi resize -> độc lập với scale màn hình
                GetClientRect(item.BindWindowHandle, out RECT bindClient);
                int bw = bindClient.Right - bindClient.Left;
                int bh = bindClient.Bottom - bindClient.Top;

                int xCenter = bw / 2;
                int y80 = (int)(bh * 0.8);
                int y20 = (int)(bh * 0.2);

                // in chẩn đoán để so sánh các hệ toạ độ ở 150% scale
                GetWindowRect(item.BindWindowHandle, out RECT bindWin);
                Console.WriteLine($"DPI top={GetDpiForWindow(item.TopWindowHandle)} bind={GetDpiForWindow(item.BindWindowHandle)}");
                Console.WriteLine($"Top  windowRect = {topHelper.GetArea()}");
                Console.WriteLine($"Bind windowRect = ({bindWin.Right - bindWin.Left} x {bindWin.Bottom - bindWin.Top})");
                Console.WriteLine($"Bind clientRect = ({bw} x {bh})");
                Console.WriteLine($"Swipe from ({xCenter}, {y80}) to ({xCenter}, {y20}) on BindWindow");

                await item.BindWindowHandle.ControlLSwipeAsync(
                    new Point(xCenter, y80),
                    new Point(xCenter, y20),
                    500,
                    10
                    );
                Console.ReadLine();
            }

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hWnd);

        struct RECT { public int Left, Top, Right, Bottom; }
    }
}
