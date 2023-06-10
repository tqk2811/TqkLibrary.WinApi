using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TqkLibrary.WinApi.PInvokeAdv.Api;

namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpyAgent<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; private set; }
        private readonly System.Windows.Forms.Timer _timer;
        private SpyAgentLocator _locator;
        private readonly int keycodeSelect;
        private readonly int keycodeExit;
        /// <summary>
        /// 
        /// </summary>
        public readonly HookKeys hookKeys = new HookKeys();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keycodeSelect"></param>
        /// <param name="keycodeExit"></param>
        public SpyAgent(int keycodeSelect = 'S', int keycodeExit = 27)
        {
            this.keycodeSelect = keycodeSelect;
            this.keycodeExit = keycodeExit;
            hookKeys.KeyCode.Add(keycodeSelect);
            hookKeys.KeyCode.Add(keycodeExit);
            hookKeys.Callback += HookKeys_Callback;
            hookKeys.HookAll = false;
            _timer = new System.Windows.Forms.Timer { Interval = 200, Enabled = false };
            _timer.Tick += OnTimerTicked;
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<SpiedWindow> SpiedWindowSelected;

        private void OnTimerTicked(object sender, EventArgs e)
        {
            ShowLocator();
        }

        private void ShowLocator()
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void BeginSpying(T t)
        {
            this.Data = t;
            _locator?.Close();
            _locator?.Dispose();
            _locator = new SpyAgentLocator();

            MakePassThrough(_locator.Handle);
            _timer.Enabled = true;
            hookKeys.SetupHook();
        }

        private void HookKeys_Callback(int keycode, bool isDown)
        {
            if (!isDown) return;
            if (SpiedWindowSelected == null) return;
            if (keycode == keycodeExit) EndSpying();
            else if (keycode == keycodeSelect) SpiedWindowSelected?.Invoke(this, GetHoveredWindow());
        }
        /// <summary>
        /// 
        /// </summary>
        public void EndSpying()
        {
            _timer.Enabled = false;
            hookKeys.UnHook();
            _locator?.Close();
            _locator?.Dispose();
            _locator = null;
            this.Data = null;
        }

        private class SpyAgentLocator : Form
        {
            public SpyAgentLocator()
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.OrangeRed;
                Opacity = 0.25;
                TopLevel = true;
                TopMost = true;
                ShowInTaskbar = false;
            }
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