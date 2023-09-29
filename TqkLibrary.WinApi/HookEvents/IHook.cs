using System;

namespace TqkLibrary.WinApi.HookEvents
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHook : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        void SetupHook();

        /// <summary>
        /// 
        /// </summary>
        void UnHook();
    }
}