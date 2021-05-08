//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TqkLibrary.WinApi.Api
//{
//  /// <summary>
//  /// PeekMessage
//  /// </summary>
//  public enum PM : uint
//  {
//    /// <summary>
//    /// Messages are not removed from the queue after processing by PeekMessage.
//    /// </summary>
//    PM_NOREMOVE = 0x0000,
//    /// <summary>
//    /// Messages are removed from the queue after processing by PeekMessage.
//    /// </summary>
//    PM_REMOVE = 0x0001,
//    /// <summary>
//    /// Prevents the system from releasing any thread that is waiting for the caller to go idle (see WaitForInputIdle).
//    /// Combine this value with either PM_NOREMOVE or PM_REMOVE.
//    /// </summary>
//    PM_NOYIELD = 0x0002
//  }
//}