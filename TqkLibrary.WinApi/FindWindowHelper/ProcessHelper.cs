using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProcessHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processId"></param>
        public ProcessHelper(uint processId)
        {
            ProcessId = processId;
        }

        /// <summary>
        /// 
        /// </summary>
        public uint ProcessId { get; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> AllWindows
        {
            get
            {
                foreach (var handle in ProcessId.GetWindowsOfProcess())
                {
                    yield return new WindowHelper(handle);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<WindowHelper> WindowsTree
        {
            get
            {
                foreach (HWND handle in ProcessId.GetWindowsOfProcess())
                {
                    HWND parentHandle = PInvoke.GetParent(handle);
                    if (parentHandle == IntPtr.Zero)
                    {
                        yield return new WindowHelper(handle);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProcessHelper? ParentProcess
        {
            get
            {
                //TH32CS_SNAPPROCESS will capture all process
                using SafeHandle snapshot = PInvoke.CreateToolhelp32Snapshot_SafeHandle(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
                if (!snapshot.IsInvalid)
                {
                    PROCESSENTRY32 pe = new PROCESSENTRY32();
                    pe.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
                    if (PInvoke.Process32First(snapshot, ref pe))
                    {
                        do
                        {
                            if (pe.th32ProcessID == ProcessId)
                            {
                                return new ProcessHelper(pe.th32ParentProcessID);
                            }
                        } while (PInvoke.Process32Next(snapshot, ref pe));
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ProcessHelper> ChildrensProcess
        {
            get
            {
                //TH32CS_SNAPPROCESS will capture all process
                using SafeHandle snapshot = PInvoke.CreateToolhelp32Snapshot_SafeHandle(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
                if (!snapshot.IsInvalid)
                {
                    PROCESSENTRY32 pe = new PROCESSENTRY32();
                    pe.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
                    if (PInvoke.Process32First(snapshot, ref pe))
                    {
                        do
                        {
                            if (pe.th32ParentProcessID == ProcessId)
                            {
                                yield return new ProcessHelper(pe.th32ProcessID);
                            }
                        } while (PInvoke.Process32Next(snapshot, ref pe));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal SafeHandle GetProcessHandle(PROCESS_ACCESS_RIGHTS standardRight = PROCESS_ACCESS_RIGHTS.PROCESS_SYNCHRONIZE)
        {
            return PInvoke.OpenProcess_SafeHandle(standardRight, true, ProcessId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is ProcessHelper processHelper)
            {
                return processHelper.ProcessId == ProcessId;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ProcessId.GetHashCode();
        }


        /// <summary>
        /// 
        /// </summary>
        public static IReadOnlyList<ProcessHelper> AllProcesses
        {
            get
            {
                return System.Diagnostics.Process.GetProcesses()
                    .Select(x => new ProcessHelper((uint)x.Id))
                    .ToList();
            }
        }
    }
}
