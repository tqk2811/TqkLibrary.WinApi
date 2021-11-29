using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr BindWindowHandle = new IntPtr(0xc077e);//BindWindowHandle
            IntPtr TopWindowHandle = new IntPtr(0x909e0);//TopWindowHandle
            LdPlayerHelper ldPlayerHelper = new LdPlayerHelper(TopWindowHandle, BindWindowHandle);
            while (true)
            {
                //BindWindowHandle.ControlLClick(220, 65);
                //foreach(char c in "TeSt23")
                //{
                //  BindWindowHandle.SendKey(c);
                //}

                ldPlayerHelper.ScreenShot(ScreenShotType.BitBlt).Save("D:\\test.png");

                Console.ReadLine();
            }

        }
    }
}
