using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Foundation;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace TqkLibrary.WinApi.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class MonitorHelper
    {
        internal unsafe static IEnumerable<HMONITOR> Monitors
        {
            get
            {
                List<HMONITOR> monitors = new List<HMONITOR>();
                GCHandle gch = GCHandle.Alloc(monitors);
                try
                {
                    RECT? rect = null;
                    MONITORENUMPROC proc = MonitorCallback;
                    BOOL bOOL = Windows.Win32.PInvoke.EnumDisplayMonitors(new HDC(IntPtr.Zero), rect, proc, GCHandle.ToIntPtr(gch));
                }
                finally
                {
                    if (gch.IsAllocated)
                        gch.Free();
                }
                return monitors;
            }
        }
        unsafe static BOOL MonitorCallback(HMONITOR param0, HDC param1, RECT* param2, LPARAM param3)
        {
            var list = GCHandle.FromIntPtr(param3).Target as List<HMONITOR>;
            list?.Add(param0);
            return true;
        }


        internal static IEnumerable<MONITORINFO> GetMonitorsInfo(this IEnumerable<HMONITOR> hMonitors)
        {
            foreach (HMONITOR monitor in hMonitors)
            {
                MONITORINFO mONITORINFO = new MONITORINFO();
                mONITORINFO.cbSize = (uint)Marshal.SizeOf<MONITORINFO>();
                if (Windows.Win32.PInvoke.GetMonitorInfo(monitor, ref mONITORINFO))
                {
                    yield return mONITORINFO;
                }
            }
        }
        internal static IEnumerable<float> GetMonitorsScale(this IEnumerable<HMONITOR> hMonitors)
        {
            foreach (HMONITOR monitor in hMonitors)
            {
                HRESULT hresult = Windows.Win32.PInvoke.GetDpiForMonitor(monitor, Windows.Win32.UI.HiDpi.MONITOR_DPI_TYPE.MDT_DEFAULT, out uint x, out uint y);
                if (hresult == 0)
                {
                    yield return x / 96.0f;
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="appScale"></param>
        /// <param name="isChrome"></param>
        /// <returns></returns>
        public static (Size, Point) GetLocationApp(int position, int rows = 2, int cols = 2, float? appScale = null, bool isChrome = false)
        {
            var monitors = MonitorHelper.Monitors.ToList();
            var monitorsInfo = monitors.GetMonitorsInfo().ToList();

            int screenIndex = position / (rows * cols) % monitors.Count();
            int locationInScreen = position % (rows * cols);
            int x_location_InScreen = locationInScreen % cols;
            int y_location_InScreen = locationInScreen / cols;

            var monitorsScale = monitors.GetMonitorsScale().ToList();
            var screenInfo = monitorsInfo.Skip(screenIndex).First();

            var screenScale = monitorsScale.Skip(screenIndex).First();
            float _appScale = appScale.HasValue ? appScale.Value : screenScale;



            Size windowSize = new Size(
                (int)(screenInfo.rcWork.Width / cols / _appScale),
                (int)(screenInfo.rcWork.Height / rows / _appScale)
                );

            Point resultPoint = new Point(
                (int)(screenInfo.rcMonitor.left / _appScale / (isChrome ? _appScale : 1.0f)) + windowSize.Width * x_location_InScreen,
                (int)(screenInfo.rcMonitor.top / _appScale / (isChrome ? _appScale : 1.0f)) + windowSize.Height * y_location_InScreen
                );


            return (windowSize, resultPoint);
        }
    }
}
