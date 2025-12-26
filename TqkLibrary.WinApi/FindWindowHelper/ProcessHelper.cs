using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        /// <param name="processId"></param>
        public ProcessHelper(int processId)
        {
            ProcessId = (uint)processId;
        }

        const uint STILL_ACTIVE = 259;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAlive
        {
            get
            {
                using SafeHandle safeHandle = GetProcessHandle(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION);
                if (safeHandle.IsInvalid) return false;
                if (PInvoke.GetExitCodeProcess(safeHandle, out uint exitcode))
                {
                    return exitcode == STILL_ACTIVE;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProcessImageName
        {
            get
            {
                using SafeHandle safeHandle = GetProcessHandle(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION);
                uint bufferSize = 260;
                char[] buffer = ArrayPool<char>.Shared.Rent((int)bufferSize);
                try
                {
                    while (bufferSize <= 32768)
                    {
                        uint actualSize = (uint)buffer.Length;
                        if (PInvoke.QueryFullProcessImageName(safeHandle, PROCESS_NAME_FORMAT.PROCESS_NAME_WIN32, buffer, ref actualSize))
                        {
                            return new string(buffer, 0, (int)actualSize);
                        }

                        int error = Marshal.GetLastWin32Error();
                        if (error == (int)WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
                        {
                            bufferSize *= 2;
                            ArrayPool<char>.Shared.Return(buffer);
                            buffer = ArrayPool<char>.Shared.Rent((int)bufferSize);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
                return string.Empty;
            }
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
        /// <returns></returns>
        public override string ToString()
        {
            List<Tuple<string, string>> tuples = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(nameof(ProcessId),ProcessId.ToString()),
                new Tuple<string, string>(nameof(IsAlive),IsAlive.ToString()),
            };
            var _ProcessImageName = ProcessImageName;
            if (File.Exists(_ProcessImageName))
            {
                tuples.Add(new Tuple<string, string>("Name", new FileInfo(_ProcessImageName).Name));
            }
            return string.Join(",", tuples.Select(x => $"{x.Item1}: {x.Item2}"));
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<ProcessHelper> AllProcesses
        {
            get
            {
                return System.Diagnostics.Process.GetProcesses()
                    .Select(x => new ProcessHelper((uint)x.Id));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<ProcessHelper> GetProcessesByName(string name)
            => System.Diagnostics.Process.GetProcessesByName(name)
                .Select(x => new ProcessHelper((uint)x.Id));

    }
}
