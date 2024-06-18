using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TqkLibrary.WinApi
{
    public static partial class Extensions
    {
        const uint MK_LBUTTON = 0x0001;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="point"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ControlLClickAsync(this IntPtr windowHandle, Point point, int delay = 50, CancellationToken cancellationToken = default)
        {
            PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), point.ToLParam());
            await Task.Delay(delay, cancellationToken);
            PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, new WPARAM(MK_LBUTTON), point.ToLParam());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ControlLSwipeAsync(this IntPtr windowHandle,
            Point from, Point to, int duration = 500, int step = 10, CancellationToken cancellationToken = default)
        {
            int times = duration / step;

            int x_step = (to.X - from.X) / times;
            int y_step = (to.Y - from.Y) / times;

            int x = from.X;
            int y = from.Y;
            try
            {
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), CreateLParam(x, y));
                for (int i = 1; i < times; i++)
                {
                    await Task.Delay(step, cancellationToken);
                    x = from.X + i * x_step;
                    y = from.Y + i * y_step;
                    PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(x, y));
                }
                await Task.Delay(step, cancellationToken);
            }
            finally
            {
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(to.X, to.Y));
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, new WPARAM(MK_LBUTTON), CreateLParam(to.X, to.Y));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="points"></param>
        /// <param name="totalDuration"></param>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ControlLSwipeMultiPointAsync(this IntPtr windowHandle,
            IReadOnlyList<Point> points,
            int totalDuration,
            int step = 10,
            CancellationToken cancellationToken = default
            )
        {
            if (points.Count < 2)
                throw new InvalidOperationException($"{nameof(points)}.Count mustbe >= 2");
            else if (points.Count == 2)
            {
                await windowHandle.ControlLSwipeAsync(points.First(), points.Last(), totalDuration, step, cancellationToken);
                return;
            }

            double totalRange = 0.0;
            List<double> ranges = new List<double>();
            for (int i = 1; i < points.Count; i++)
            {
                double range = Math.Sqrt(Math.Pow(points[i - 1].X - points[i].X, 2) + Math.Pow(points[i - 1].Y - points[i].Y, 2));
                totalRange += range;
                ranges.Add(range);
            }

            int times = totalDuration / step;
            double rangePerStep = totalRange / times;

            List<Point> pointsMove = new List<Point>();
            for (int i = 1; i < points.Count; i++)
            {
                pointsMove.Add(points[i - 1]);
                double range = ranges[i - 1];
                int straightTimes = (int)(range / rangePerStep);
                Point vector = new Point(points[i].X - points[i - 1].X, points[i].Y - points[i - 1].Y);//vector = B - A
                //cos(vector) = X/rangePerStep => X = cos(vector) * rangePerStep = vector.X / range * rangePerStep
                //sin(vector) = Y/rangePerStep => Y = sin(vector) * rangePerStep = vector.Y / range * rangePerStep
                //point = (X,Y) + A
                for (int j = 0; j < straightTimes; j++)
                {
                    pointsMove.Add(new Point(
                        (int)(vector.X / range * rangePerStep * (j + 1)) + points[i - 1].X,
                        (int)(vector.Y / range * rangePerStep * (j + 1)) + points[i - 1].Y
                        ));
                }
            }
            pointsMove.Add(points.Last());

            try
            {
                Point first = pointsMove.First();
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONDOWN, new WPARAM(MK_LBUTTON), CreateLParam(first.X, first.Y));
                for (int i = 0; i < pointsMove.Count; i++)
                {
                    PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(pointsMove[i].X, pointsMove[i].Y));
                    await Task.Delay(step, cancellationToken);
                }
            }
            finally
            {
                Point last = pointsMove.Last();
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_MOUSEMOVE, new WPARAM(MK_LBUTTON), CreateLParam(last.X, last.Y));
                PInvoke.SendMessage((HWND)windowHandle, PInvoke.WM_LBUTTONUP, new WPARAM(MK_LBUTTON), CreateLParam(last.X, last.Y));
            }
        }
    }
}
