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
        static async Task Main(string[] args)
        {
            ProcessHelper rootProcessHelper = new ProcessHelper((uint)16760);

            List<ProcessHelper> processHelpers = new List<ProcessHelper>();
            processHelpers.Add(rootProcessHelper);
            processHelpers.AddRange(rootProcessHelper.ChildrensProcess);

            foreach (ProcessHelper processHelper in processHelpers)
            {
                foreach (WindowHelper windowHelper in processHelper.AllWindows)
                {
                    if (windowHelper.Title?.Contains("Open", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        WindowHelper? ComboBoxEx32 = windowHelper.ChildrensWindow.FirstOrDefault(x => x.ClassName.Equals("ComboBoxEx32"));
                        WindowHelper? ComboBox = ComboBoxEx32?.ChildrensWindow?.FirstOrDefault(x => x.ClassName.Equals("ComboBox"));
                        WindowHelper? Edit = ComboBox?.ChildrensWindow?.FirstOrDefault(x => x.ClassName.Equals("Edit"));
                        WindowHelper? openButton = windowHelper.ChildrensWindow.FirstOrDefault(x => x.ClassName.Equals("Button") && x.Title.Equals("&Open"));
                        if (Edit is not null && openButton is not null)
                        {
                            await Edit.WindowHandle.SendTextUnicodeAsync("C:\\Users\\tqk2811\\Pictures\\Otaku\\anemoi\\character_2.png");
                            uint BM_CLICK = 245U;
                            openButton.SendMessage(BM_CLICK, 0, 0);
                            return;
                        }
                    }
                }
            }

            ProcessHelper.Win32_Process? win32_Process = rootProcessHelper.Query_Win32_Process();










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
