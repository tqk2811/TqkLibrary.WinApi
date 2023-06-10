using System;
using System.Runtime.Serialization;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ScriptException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ScriptException()
            : base("Script Exception")
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ScriptException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public ScriptException(Exception innerException)
            : base(null, innerException)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public int Line { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public int Column { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public int Number { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; internal set; }
    }
}