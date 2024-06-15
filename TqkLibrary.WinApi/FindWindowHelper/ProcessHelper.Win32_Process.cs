using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TqkLibrary.WinApi.FindWindowHelper
{
    public partial class ProcessHelper
    {
        static readonly Regex regex_windowDateTime = new Regex("(\\d{14}\\.\\d{6})([+-]{1}\\d+)$");

        static readonly IReadOnlyDictionary<Type, Action<Win32_Process, PropertyInfo, string>> dict_Win32_Process
            = new Dictionary<Type, Action<Win32_Process, PropertyInfo, string>>()
        {
            {
                typeof(string),
                (win32_Process,propertyInfo,value) =>
                {
                    propertyInfo.SetValue(win32_Process, value);
                }
            },
            {
                typeof(Nullable<UInt16>),
                (win32_Process,propertyInfo,value) =>
                {
                    if(UInt16.TryParse(value,out UInt16 v))
                    {
                        propertyInfo.SetValue(win32_Process, v);
                    }
                }
            },
            {
                typeof(Nullable<UInt32>),
                (win32_Process,propertyInfo,value) =>
                {
                    if(UInt32.TryParse(value,out UInt32 v))
                    {
                        propertyInfo.SetValue(win32_Process, v);
                    }
                }
            },
            {
                typeof(Nullable<UInt64>),
                (win32_Process,propertyInfo,value) =>
                {
                    if(UInt64.TryParse(value,out UInt64 v))
                    {
                        propertyInfo.SetValue(win32_Process, v);
                    }
                }
            },
            {
                typeof(Nullable<DateTime>),
                (win32_Process,propertyInfo,value) =>
                {
                    //20240615101842.546420+420
                    Match match = regex_windowDateTime.Match(value);
                    if(match.Success)
                    {
                        if(int.TryParse(match.Groups[2].Value,out int minuteOffset))
                        {
                            int hour = minuteOffset / 60;
                            string _hour = minuteOffset >= 0 ? $"+{hour:00}" : hour.ToString("00");
                            int min = minuteOffset % 60;

                            if(DateTime.TryParseExact(
                                $"{match.Groups[1].Value}{_hour}:{min:00}",
                                "yyyyMMddHHmmss.ffffffzzz",
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out DateTime v))
                            {
                                propertyInfo.SetValue(win32_Process, v.AddMinutes(minuteOffset));
                            }
                        }
                    }
                }
            },
        };


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
                PropertyInfo[] properties = typeof(Win32_Process).GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    string? value = mo[propertyInfo.Name]?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (dict_Win32_Process.ContainsKey(propertyInfo.PropertyType))
                        {
                            dict_Win32_Process[propertyInfo.PropertyType].Invoke(win32_Process, propertyInfo, value!);
                        }
                    }
                }
                return win32_Process;
            }
            return null;
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process
        /// </summary>
        public class Win32_Process
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
}
