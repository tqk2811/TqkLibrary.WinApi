using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class HardWareId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetVolumeSerialNumber()
        {
            ManagementObject dsk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            dsk.Get();
            return dsk["VolumeSerialNumber"].ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorID()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
            {
                return mo["ProcessorId"].ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetMainboardSerial()
        {
            var mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
            ManagementObjectCollection mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
            {
                return mo["SerialNumber"].ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string CalcHashVolumeSerialNumber(string salt)
        {
            string volumeserial = GetVolumeSerialNumber();
            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] bytes = sHA256.ComputeHash(Encoding.ASCII.GetBytes($"{volumeserial}|{salt}"));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}