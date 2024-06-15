using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Enums;
using TqkLibrary.WinApi.FindWindowHelper;
using TqkLibrary.WinApi.HandleControls;

namespace TqkLibrary.WinApi.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessHelper processHelper = Process.GetProcessesByName("chrome").Select(x => new ProcessHelper((uint)x.Id)).First();
            ProcessHelper.Win32_Process? win32_Process = processHelper.Query_Win32_Process();










            IntPtr BindWindowHandle = new IntPtr(0xc077e);//BindWindowHandle
            IntPtr TopWindowHandle = new IntPtr(0x909e0);//TopWindowHandle
            LdPlayerAndroidEmulatorHelper ldPlayerHelper = new LdPlayerAndroidEmulatorHelper(TopWindowHandle, BindWindowHandle);
            while (true)
            {
                //BindWindowHandle.ControlLClick(220, 65);
                //foreach(char c in "TeSt23")
                //{
                //  BindWindowHandle.SendKey(c);
                //}

                //ldPlayerHelper.ScreenShot(CaptureType.BitBlt).Save("D:\\test.png");

                Console.ReadLine();
            }

        }
    }
}
