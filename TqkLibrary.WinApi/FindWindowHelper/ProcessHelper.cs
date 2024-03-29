﻿using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
        /// <param name="processId"></param>
        public ProcessHelper(int processId)
        {
            this.ProcessId = processId;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ProcessId { get; }

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
                foreach (var handle in ProcessId.GetWindowsOfProcess())
                {
                    IntPtr parentHandle = User32.GetParent(handle);
                    if(parentHandle == IntPtr.Zero)
                    {
                        yield return new WindowHelper(handle);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProcessHelper ParentProcess
        {
            get
            {
                using var snapshot = Kernel32.CreateToolhelp32Snapshot(Kernel32.CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0);//TH32CS_SNAPPROCESS will capture all process
                if (snapshot.IsInvalid)
                {
                    var err = Kernel32.GetLastError();
                    if (err != Win32ErrorCode.ERROR_SUCCESS)
                        throw new Win32Exception(err);
                }
                Kernel32.PROCESSENTRY32 pe = new Kernel32.PROCESSENTRY32();
                if (Kernel32.Process32First(snapshot, ref pe))
                {
                    do
                    {
                        if (pe.th32ProcessID == ProcessId)
                        {
                            return new ProcessHelper(pe.th32ParentProcessID);
                        }
                    } while (Kernel32.Process32Next(snapshot, ref pe));
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
                using var snapshot = Kernel32.CreateToolhelp32Snapshot(Kernel32.CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0);//TH32CS_SNAPPROCESS will capture all process
                if (snapshot.IsInvalid)
                {
                    var err = Kernel32.GetLastError();
                    if (err != Win32ErrorCode.ERROR_SUCCESS)
                        throw new Win32Exception(err);
                }
                Kernel32.PROCESSENTRY32 pe = new Kernel32.PROCESSENTRY32();
                if (Kernel32.Process32First(snapshot, ref pe))
                {
                    do
                    {
                        if (pe.th32ParentProcessID == ProcessId)
                        {
                            yield return new ProcessHelper(pe.th32ProcessID);
                        }
                    } while (Kernel32.Process32Next(snapshot, ref pe));
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public Kernel32.SafeObjectHandle GetProcessHandle(Kernel32.ACCESS_MASK.StandardRight standardRight = Kernel32.ACCESS_MASK.StandardRight.SYNCHRONIZE)
        {
            return Kernel32.OpenProcess(Kernel32.ACCESS_MASK.StandardRight.SYNCHRONIZE, true, this.ProcessId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ProcessHelper processHelper)
            {
                return processHelper.ProcessId == this.ProcessId;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ProcessId.GetHashCode();
        }
    }
}
