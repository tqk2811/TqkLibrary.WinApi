using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi.WmiHelpers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseWmiData
    {
        static readonly Regex regex_windowDateTime = new Regex("(\\d{14}\\.\\d{6})([+-]{1}\\d+)$");

        static readonly IReadOnlyDictionary<Type, Action<object, PropertyInfo, string>> dict_Win32_Process
            = new Dictionary<Type, Action<object, PropertyInfo, string>>()
        {
            {
                typeof(string),
                (instance, propertyInfo, value) =>
                {
                    propertyInfo.SetValue(instance, value);
                }
            },
            {
                typeof(Nullable<UInt16>),
                (instance, propertyInfo, value) =>
                {
                    if(UInt16.TryParse(value,out UInt16 v))
                    {
                        propertyInfo.SetValue(instance, v);
                    }
                }
            },
            {
                typeof(Nullable<UInt32>),
                (instance, propertyInfo, value) =>
                {
                    if(UInt32.TryParse(value,out UInt32 v))
                    {
                        propertyInfo.SetValue(instance, v);
                    }
                }
            },
            {
                typeof(Nullable<UInt64>),
                (instance, propertyInfo, value) =>
                {
                    if(UInt64.TryParse(value,out UInt64 v))
                    {
                        propertyInfo.SetValue(instance, v);
                    }
                }
            },
            {
                typeof(Nullable<DateTime>),
                (instance, propertyInfo, value) =>
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
                                propertyInfo.SetValue(instance, v.AddMinutes(minuteOffset));
                            }
                        }
                    }
                }
            },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementObject"></param>
        public void Parse(ManagementObject managementObject)
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string? value = managementObject[propertyInfo.Name]?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (dict_Win32_Process.ContainsKey(propertyInfo.PropertyType))
                    {
                        dict_Win32_Process[propertyInfo.PropertyType].Invoke(this, propertyInfo, value!);
                    }
                }
            }
        }
    }
}
