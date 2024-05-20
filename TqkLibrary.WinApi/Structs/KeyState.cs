namespace TqkLibrary.WinApi.Structs
{
    /// <summary>
    /// 
    /// </summary>
    public struct KeyState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public KeyState(short val)
        {
            Val = val;
        }
        /// <summary>
        /// 
        /// </summary>
        public short Val;

        /// <summary>
        /// 
        /// </summary>
        public bool IsDown { get { return (Val >> 8 & 0x1) == 0x1; } }
        /// <summary>
        /// 
        /// </summary>
        public bool IsToggled { get { return (Val & 0x1) == 0x1; } }
    }
}
