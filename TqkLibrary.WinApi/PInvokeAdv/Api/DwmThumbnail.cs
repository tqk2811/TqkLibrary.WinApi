using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class DwmThumbnail : IDisposable
    {
        readonly IntPtr _handleTarget;
        readonly IntPtr _targetWindowHandle;
        IntPtr _hThumbnailId;
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public DwmThumbnail(IntPtr handleTarget, IntPtr targetWindowHandle)
        {
            if (targetWindowHandle == IntPtr.Zero) throw new ArgumentNullException(nameof(targetWindowHandle));
            if (handleTarget == IntPtr.Zero) throw new ArgumentNullException(nameof(handleTarget));
            this._handleTarget = handleTarget;
            this._targetWindowHandle = targetWindowHandle;
        }
        /// <summary>
        /// 
        /// </summary>
        ~DwmThumbnail()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_hThumbnailId != IntPtr.Zero)
            {
                NativeWrapper.DwmUnregisterThumbnail(_hThumbnailId);
                _hThumbnailId = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Register()
        {
            if (_hThumbnailId == IntPtr.Zero)
            {
                bool result = NativeWrapper.DwmRegisterThumbnail(_handleTarget, _targetWindowHandle, ref _hThumbnailId) >= 0;
                return result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Update(DWM_THUMBNAIL_PROPERTIES dWM_THUMBNAIL_PROPERTIES)
        {
            if (_hThumbnailId != IntPtr.Zero)
            {
                bool result = NativeWrapper.DwmUpdateThumbnailProperties(_hThumbnailId, ref dWM_THUMBNAIL_PROPERTIES) >= 0;

                return result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SIZE DwmQueryThumbnailSourceSize()
        {
            SIZE size = new SIZE();
            if (_hThumbnailId != IntPtr.Zero)
            {
                NativeWrapper.DwmQueryThumbnailSourceSize(_hThumbnailId, ref size);
            }
            return size;
        }

        #region Native
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum DWM_TNP : UInt32
        {
            /// <summary>
            /// A value for the rcDestination member has been specified.
            /// </summary>
            DWM_TNP_RECTDESTINATION = 0x00000001,
            /// <summary>
            /// A value for the rcSource member has been specified.
            /// </summary>
            DWM_TNP_RECTSOURCE = 0x00000002,
            /// <summary>
            /// A value for the opacity member has been specified.
            /// </summary>
            DWM_TNP_OPACITY = 0x00000004,
            /// <summary>
            /// A value for the fVisible member has been specified.
            /// </summary>
            DWM_TNP_VISIBLE = 0x00000008,
            /// <summary>
            /// A value for the fSourceClientAreaOnly member has been specified.
            /// </summary>
            DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010
        }
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RECT
        {
            /// <summary>
            /// 
            /// </summary>
            public int left;
            /// <summary>
            /// 
            /// </summary>
            public int top;
            /// <summary>
            /// 
            /// </summary>
            public int right;
            /// <summary>
            /// 
            /// </summary>
            public int bottom;
        }
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            /// <summary>
            /// 
            /// </summary>
            public DWM_TNP dwFlags;
            /// <summary>
            /// 
            /// </summary>
            public RECT rcDestination;
            /// <summary>
            /// 
            /// </summary>
            public RECT rcSource;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte opacity;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool fVisible;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool fSourceClientAreaOnly;
        }
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SIZE
        {
            /// <summary>
            /// 
            /// </summary>
            public UInt32 cx;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 cy;
        }

        static class NativeWrapper
        {
            [DllImport("Dwmapi.dll", CallingConvention = CallingConvention.Winapi)]
            public static extern long DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource, ref IntPtr phThumbnailId);

            [DllImport("Dwmapi.dll", CallingConvention = CallingConvention.Winapi)]
            public static extern long DwmUpdateThumbnailProperties(IntPtr hThumbnailId, ref DWM_THUMBNAIL_PROPERTIES ptnProperties);

            [DllImport("Dwmapi.dll", CallingConvention = CallingConvention.Winapi)]
            public static extern long DwmQueryThumbnailSourceSize(IntPtr hThumbnail, ref SIZE pSize);

            [DllImport("Dwmapi.dll", CallingConvention = CallingConvention.Winapi)]
            public static extern long DwmUnregisterThumbnail(IntPtr hThumbnailId);
        }
        #endregion
    }
}
