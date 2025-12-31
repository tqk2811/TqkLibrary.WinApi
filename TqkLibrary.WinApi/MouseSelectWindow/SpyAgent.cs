using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.WinApi.Helpers;
using TqkLibrary.WinApi.HookEvents;

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
        public event EventHandler<WindowHelper?>? OnWindowSelected;

        private readonly SynchronizationContext _synchronizationContext;
        private MouseHook? _mouseHook;
        private SpyAgentLocator? _locator;

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
            _synchronizationContext = synchronizationContext;
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
                MakePassThrough((HWND)_locator.Handle);

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
                case PInvoke.WM_MOUSEMOVE:
                    ShowLocator();
                    break;

                case PInvoke.WM_LBUTTONDOWN:
                    try { await EndSpyingAsync(); } catch { }
                    OnWindowSelected?.Invoke(this, GetHoveredWindow());
                    break;

                case PInvoke.WM_RBUTTONDOWN:
                    EndSpying();
                    break;
            }
        }

        private void ShowLocator()
        {
            _synchronizationContext.Post((o) =>
            {
                WindowHelper window = GetHoveredWindow();
                if (window.WindowHandle == IntPtr.Zero || !window.Area.HasValue)
                {
                    _locator?.Hide();
                    return;
                }
                if (_locator is not null)
                {
                    _locator.Location = window.Area.Value.Location;
                    _locator.Size = window.Area.Value.Size;
                    _locator.TopLevel = true;
                    _locator.TopMost = true;
                    _locator.Show();
                }
            },
            null);
        }

        #region Under the Hood
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WindowHelper? GetHoveredWindow()
        {
            if (PInvoke.GetCursorPos(out Point point))
            {
                HWND handle = PInvoke.WindowFromPoint(point);
                return new WindowHelper(handle);
            }
            return null;
        }

        const long WS_EX_TRANSPARENT = 0x00000020L;
        private static void MakePassThrough(HWND handle)
        {
            int exstyle = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            PInvoke.SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)WS_EX_TRANSPARENT | exstyle);
        }
        #endregion Under the Hood
    }
}