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
      IntPtr handle = new IntPtr(5377338);//BindWindowHandle
      //

      IntPtr handle2 = new IntPtr(985282);//TopWindowHandle
      while (true)
      {
        //handle.ControlLClick(220, 65);
        foreach(char c in "TeSt23")
        {
          handle.SendKey(c);
        }
        
        //handle2.CaptureWindow().Save("D:\\test.png");

        Console.ReadLine();
      }
      
    }
  }
}
