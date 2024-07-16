using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TqkLibrary.WinApi.WmiHelpers
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process
    /// </summary>
    public class Win32_Process : BaseWmiData
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public string? CreationClassName { get; set; }
        public string? Caption { get; set; }
        public string? CommandLine { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? CSCreationClassName { get; set; }
        public string? CSName { get; set; }
        public string? Description { get; set; }
        public string? ExecutablePath { get; set; }
        public UInt16? ExecutionState { get; set; }
        public string? Handle { get; set; }
        public UInt32? HandleCount { get; set; }
        public DateTime? InstallDate { get; set; }
        public UInt64? KernelModeTime { get; set; }
        public UInt32? MaximumWorkingSetSize { get; set; }
        public UInt32? MinimumWorkingSetSize { get; set; }
        public string? Name { get; set; }
        public string? OSCreationClassName { get; set; }
        public string? OSName { get; set; }
        public UInt64? OtherOperationCount { get; set; }
        public UInt64? OtherTransferCount { get; set; }
        public UInt32? PageFaults { get; set; }
        public UInt32? PageFileUsage { get; set; }
        public UInt32? ParentProcessId { get; set; }
        public UInt32? PeakPageFileUsage { get; set; }
        public UInt64? PeakVirtualSize { get; set; }
        public UInt32? PeakWorkingSetSize { get; set; }
        public UInt32? Priority { get; set; }
        public UInt64? PrivatePageCount { get; set; }
        public UInt32? ProcessId { get; set; }
        public UInt32? QuotaNonPagedPoolUsage { get; set; }
        public UInt32? QuotaPagedPoolUsage { get; set; }
        public UInt32? QuotaPeakNonPagedPoolUsage { get; set; }
        public UInt32? QuotaPeakPagedPoolUsage { get; set; }
        public UInt64? ReadOperationCount { get; set; }
        public UInt64? ReadTransferCount { get; set; }
        public UInt32? SessionId { get; set; }
        public string? Status { get; set; }
        public DateTime? TerminationDate { get; set; }
        public UInt32? ThreadCount { get; set; }
        public UInt64? UserModeTime { get; set; }
        public UInt64? VirtualSize { get; set; }
        public string? WindowsVersion { get; set; }
        public UInt64? WorkingSetSize { get; set; }
        public UInt64? WriteOperationCount { get; set; }
        public UInt64? WriteTransferCount { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
