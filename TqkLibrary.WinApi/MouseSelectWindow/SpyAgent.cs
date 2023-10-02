using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TqkLibrary.WinApi.HookEvents;
using TqkLibrary.WinApi.PInvokeAdv.Api;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpyAgent : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<SpiedWindow> SpiedWindowSelected;

        private readonly SynchronizationContext _synchronizationContext;
        private MouseHook _mouseHook;
        private SpyAgentLocator _locator;

        /// <summary>
        /// 
        /// </summary>
        public SpyAgent() : this(SynchronizationContext.Current)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new InvalidOperationException($"Must be calling on STA thread");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="synchronizationContext">requied for main thread sta</param>
        public SpyAgent(SynchronizationContext synchronizationContext)
        {
            this._synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// 
        /// </summary>
        ~SpyAgent()
        {
            EndSpying();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            EndSpying();
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void BeginSpying()
        {
            _synchronizationContext.Post((o) =>
            {
                _locator?.Close();
                _locator?.Dispose();
                _locator = new SpyAgentLocator();
                MakePassThrough(_locator.Handle);

                _mouseHook?.Dispose();
                _mouseHook = new MouseHook();
                _mouseHook.MouseAction += _mouseHook_MouseAction;
                _mouseHook.SetupHook();
            },
            null);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndSpying()
        {
            _synchronizationContext.Post((o) =>
            {
                _locator?.Close();
                _locator?.Dispose();
                _locator = null;

                _mouseHook?.Dispose();
                _mouseHook = null;
            },
            null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task EndSpyingAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _synchronizationContext.Post((o) =>
            {
                try
                {
                    _locator?.Close();
                    _locator?.Dispose();
                    _locator = null;

                    _mouseHook?.Dispose();
                    _mouseHook = null;

                    tcs.TrySetResult(true);
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, null);
            return tcs.Task;
        }

        private async void _mouseHook_MouseAction(object sender, MouseHook.RawMouseEventArgs e)
        {
            switch (e.Message)
            {
                case User32.WindowMessage.WM_MOUSEMOVE:
                    ShowLocator();
                    break;

                case User32.WindowMessage.WM_LBUTTONDOWN:
                    try { await EndSpyingAsync(); } catch { }
                    SpiedWindowSelected?.Invoke(this, GetHoveredWindow());
                    break;

                case User32.WindowMessage.WM_RBUTTONDOWN:
                    EndSpying();
                    break;
            }
        }

        private void ShowLocator()
        {
            _synchronizationContext.Post((o) =>
            {
                SpiedWindow window = GetHoveredWindow();
                if (window.Handle == IntPtr.Zero)
                {
                    _locator.Hide();
                    return;
                }
                _locator.Location = window.Area.Location;
                _locator.Size = window.Area.Size;
                _locator.TopLevel = true;
                _locator.TopMost = true;
                _locator.Show();
            },
            null);
        }

        #region Under the Hood
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SpiedWindow GetHoveredWindow()
        {
            IntPtr handle = User32.WindowFromPoint(Cursor.Position);
            return new SpiedWindow(handle);
        }

        private static void MakePassThrough(IntPtr handle)
        {
            int exstyle = User32.GetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
            User32.SetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, User32.SetWindowLongFlags.WS_EX_TRANSPARENT | (User32.SetWindowLongFlags)exstyle);
        }
        #endregion Under the Hood
    }
}