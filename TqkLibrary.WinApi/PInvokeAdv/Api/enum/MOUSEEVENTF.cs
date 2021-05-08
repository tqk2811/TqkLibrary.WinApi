//namespace TqkLibrary.WinApi.Api
//{
//  public enum MOUSEEVENTF : uint
//  {
//    /// <summary>
//    /// The dx and dy parameters contain normalized absolute coordinates.
//    /// If not set, those parameters contain relative data: the change in position since the last reported position.
//    /// This flag can be set, or not set, regardless of what kind of mouse or mouse-like device, if any, is connected to the system.
//    /// For further information about relative mouse motion, see the following Remarks section.
//    /// </summary>
//    ABSOLUTE = 0x8000,

//    /// <summary>
//    /// The left button is down.
//    /// </summary>
//    LEFTDOWN = 0x0002,

//    /// <summary>
//    /// The left button is up.
//    /// </summary>
//    LEFTUP = 0x0004,

//    /// <summary>
//    /// The middle button is down.
//    /// </summary>
//    MIDDLEDOWN = 0x0020,

//    /// <summary>
//    /// The middle button is up.
//    /// </summary>
//    MIDDLEUP = 0x0040,

//    /// <summary>
//    /// Movement occurred.
//    /// </summary>
//    MOVE = 0x0001,

//    /// <summary>
//    /// The right button is down.
//    /// </summary>
//    RIGHTDOWN = 0x0008,

//    /// <summary>
//    /// The right button is up.
//    /// </summary>
//    RIGHTUP = 0x0010,

//    /// <summary>
//    /// The wheel has been moved, if the mouse has a wheel.
//    /// The amount of movement is specified in dwData
//    ///
//    /// The wheel button is rotated.
//    /// </summary>
//    WHEEL = 0x0800,

//    /// <summary>
//    /// An X button was pressed.
//    /// </summary>
//    XDOWN = 0x0080,

//    /// <summary>
//    /// An X button was released.
//    /// </summary>
//    XUP = 0x0100,

//    /// <summary>
//    /// The wheel button is tilted.
//    /// </summary>
//    HWHEEL = 0x01000
//  }
//}