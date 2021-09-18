using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TqkLibrary.WinApi;
namespace TqkLibrary.WinApi.Test
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      IntPtr handle = new IntPtr(4525084);
      handle.ControlLClick(240, 155);
    }
  }
}
