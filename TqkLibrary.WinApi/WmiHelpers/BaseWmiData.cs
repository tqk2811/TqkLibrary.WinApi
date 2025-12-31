using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public abstract class BaseWmiData
    {
        static readonly Regex regex_windowDateTime = new Regex("(\\d{14}\\.\\d{6})([+-]{1}\\d+)$");

        static readonly IReadOnlyDictionary<Type, Func<object, PropertyInfo, object, bool>> dict_ParseHelpers
            = new Dictionary<Type, Func<object, PropertyInfo, object, bool>>()
        {
            {
                typeof(Nullable<DateTime>),
                (instance, propertyInfo, value) =>
                {
                    string value_str = value as string ?? value.ToString()!;
                    //20240615101842.546420+420
                    Match match = regex_windowDateTime.Match(value_str);
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
                                propertyInfo.SetValue(instance, v);
                                return true;
                            }
                        }
                    }
                    return false;
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
                foreach(PropertyData propertyData in managementObject.Properties)
                {
                    if(propertyData.Name.Equals(propertyInfo.Name))
                    {
                        object? value = propertyData.Value;
                        if (value is not null)
                        {
                            Type value_type = value.GetType();
                            if (value_type.Equals(propertyInfo.PropertyType))
                            {
                                propertyInfo.SetValue(this, value);
                                break;
                            }
                            else
                            {
                                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GenericTypeArguments.Length == 1)
                                {
                                    //Nullable<T>, IEnumerable<T>
                                    Type? NullableUnderlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                                    if (NullableUnderlyingType is not null)//Nullable<T>
                                    {
                                        if (value_type.Equals(NullableUnderlyingType))
                                        {
                                            propertyInfo.SetValue(this, value);
                                            break;
                                        }
                                    }
                                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType.IsInterface)//Array
                                    {
                                        propertyInfo.SetValue(this, value);
                                        break;
                                    }
                                }
                            }
                            if (dict_ParseHelpers.ContainsKey(propertyInfo.PropertyType))
                            {
                                if (dict_ParseHelpers[propertyInfo.PropertyType].Invoke(this, propertyInfo, value))
                                {
                                    break;
                                }
                            }

                            Debug.WriteLine($"Can't set value ({value_type.FullName}) into property ({propertyInfo.PropertyType.FullName})");
                        }
                    }
                }
            }
        }
    }
}
