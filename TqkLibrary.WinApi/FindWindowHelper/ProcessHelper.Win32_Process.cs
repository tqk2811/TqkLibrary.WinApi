using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using TqkLibrary.WinApi.WmiHelpers;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    public partial class ProcessHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Win32_Process? Query_Win32_Process()
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher($"select * from Win32_Process where ProcessId='{ProcessId}'");
            using ManagementObjectCollection moCollection = searcher.Get();
            using ManagementObject? mo = moCollection.OfType<ManagementObject>().FirstOrDefault();

            if (mo is not null)
            {
                Win32_Process win32_Process = new Win32_Process();
                win32_Process.Parse(mo);
                return win32_Process;
            }
            return null;
        }
    }
}
