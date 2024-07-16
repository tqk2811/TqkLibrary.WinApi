using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Reflection;

namespace TqkLibrary.WinApi.WmiHelpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class BaseWmiDataQueryHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IncludeHelper<T> CreateQuery<T>() where T : BaseWmiData, new()
        {
            return new IncludeHelper<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class IncludeHelper<T> where T : BaseWmiData, new()
        {
            readonly List<string> _properties = new List<string>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="selectors"></param>
            /// <returns></returns>
            public IncludeHelper<T> Include(params Expression<Func<T, object?>>[] selectors)
            {
                foreach (Expression<Func<T, object?>> selector in selectors)
                {
                    MemberExpression? memberExpression = selector.Body as MemberExpression;
                    PropertyInfo? propertyInfo = memberExpression?.Member as PropertyInfo;
                    if (propertyInfo is not null)
                    {
                        _properties.Add(propertyInfo.Name);
                    }
                }
                return this;
            }

#if DEBUG
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IncludeHelper<T> Where(Expression<Func<T, bool>> selector)
            {
                MemberExpression? memberExpression = selector.Body as MemberExpression;


                return this;
            }
#endif

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerable<T> Query()
            {
                string select = "*";
                if (_properties.Count > 0) select = string.Join(",", _properties);
                string query = $"SELECT {select} FROM {typeof(T).Name}";
                using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                using ManagementObjectCollection moCollection = searcher.Get();
                foreach (ManagementObject mo in moCollection.OfType<ManagementObject>())
                {
                    T t = new T();
                    t.Parse(mo);
                    yield return t;
                }
            }
        }
    }
}
