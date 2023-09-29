using PInvoke;
using System;
using System.Collections.Generic;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public IntPtr ProcessHandle { get; }
        /// <summary>
        /// 
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IntPtr> AllWindowHandles { get { return ProcessId.GetWindowsOfProcess(); } }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> AllWindowHelperHandles
        {
            get
            {
                foreach(var handle in AllWindowHandles)
                {
                    yield return new WindowHelper(handle);
                }
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public ProcessHelper(IntPtr handle)
        {
            this.ProcessHandle = handle;
            ProcessId = Kernel32.GetProcessId(handle);
        }




    }
}
