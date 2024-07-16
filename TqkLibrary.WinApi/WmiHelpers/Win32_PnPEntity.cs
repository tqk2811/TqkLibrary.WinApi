using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi.WmiHelpers
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-pnpentity
    /// </summary>
    public class Win32_PnPEntity : BaseWmiData
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public UInt16? Availability { get; set; }
        public string? Caption { get; set; }
        public string? ClassGuid { get; set; }
        public IReadOnlyList<string?>? CompatibleID { get; set; }
        public UInt32? ConfigManagerErrorCode { get; set; }
        public bool? ConfigManagerUserConfig { get; set; }
        public string? CreationClassName { get; set; }
        public string? Description { get; set; }
        public string? DeviceID { get; set; }
        public bool? ErrorCleared { get; set; }
        public string? ErrorDescription { get; set; }
        public IReadOnlyList<string?>? HardwareID { get; set; }
        public DateTime? InstallDate { get; set; }
        public UInt32? LastErrorCode { get; set; }
        public string? Manufacturer { get; set; }
        public string? Name { get; set; }
        public string? PNPClass { get; set; }
        public string? PNPDeviceID { get; set; }
        public IReadOnlyList<UInt16>? PowerManagementCapabilities { get; set; }
        public bool? PowerManagementSupported { get; set; }
        public bool? Present { get; set; }
        public string? Service { get; set; }
        public string? Status { get; set; }
        public UInt16? StatusInfo { get; set; }
        public string? SystemCreationClassName { get; set; }
        public string? SystemName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Win32_PnPEntity> Query()
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity");
            using ManagementObjectCollection moCollection = searcher.Get();
            foreach (ManagementObject mo in moCollection.OfType<ManagementObject>())
            {
                Win32_PnPEntity win32_PnPEntity = new Win32_PnPEntity();
                win32_PnPEntity.Parse(mo);
                yield return win32_PnPEntity;
            }
        }
    }
}
