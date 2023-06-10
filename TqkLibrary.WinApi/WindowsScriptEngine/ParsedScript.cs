using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ParsedScript : IDisposable
    {
        private object _dispatch;
        private readonly ScriptEngine _engine;

        internal ParsedScript(ScriptEngine engine, IntPtr dispatch)
        {
            _engine = engine;
            _dispatch = Marshal.GetObjectForIUnknown(dispatch);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public object CallMethod(string methodName, params object[] arguments)
        {
            if (_dispatch == null)
                throw new InvalidOperationException();

            if (methodName == null)
                throw new ArgumentNullException("methodName");

            try
            {
                return _dispatch.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, _dispatch, arguments);
            }
            catch
            {
                if (_engine.Site.LastException != null)
                    throw _engine.Site.LastException;

                throw;
            }
        }

        void IDisposable.Dispose()
        {
            if (_dispatch != null)
            {
                Marshal.ReleaseComObject(_dispatch);
                _dispatch = null;
            }
        }
    }
}