using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TqkLibrary.WinApi.PInvokeAdv.Api;

namespace TqkLibrary.WinApi
{
  public class SpiedWindow : EventArgs
  {
    public SpiedWindow(IntPtr handle)
    {
      Handle = handle;
      User32.GetWindowRect(Handle, out RECT rect);
      Area = rect.GetRectangle();
      SetCaption();
    }

    public IntPtr Handle { get; private set; }
    public Rectangle Area { get; private set; }
    public string Caption { get; private set; }

    private void SetCaption()
    {
      int len = User32.GetWindowTextLength(Handle);
      var str = new StringBuilder(len + 1);
      User32.GetWindowText(Handle, str.ToString().ToCharArray(), len + 1);
      Caption = str.ToString();
    }

    public SpiedWindow GetParentWindow()
    {
      return new SpiedWindow(MyUser32.GetParent(Handle));
    }

    public void SeParentwindow(IntPtr parentHandle)
    {
      User32.SetParent(Handle, parentHandle);
    }

    public IEnumerable<SpiedWindow> GetChildren()
    {
      var children = new List<SpiedWindow>();
      MyUser32.EnumChildWindows(Handle, (hWnd, lp) =>
      {
        children.Add(new SpiedWindow(hWnd));
        return true;
      }, IntPtr.Zero);
      return children;
    }

    public override string ToString()
    {
      return Caption;
    }
  }

  public class SpyAgent
  {
    private readonly System.Windows.Forms.Timer _timer;
    private SpyAgentLocator _locator;
    private readonly int keycodeSelect;
    private readonly int keycodeExit;
    public readonly HookKeys hookKeys = new HookKeys();

    public SpyAgent(int keycodeSelect = 'S', int keycodeExit = 27)
    {
      this.keycodeSelect = keycodeSelect;
      this.keycodeExit = keycodeExit;
      hookKeys.KeyCode.Add(keycodeSelect);
      hookKeys.KeyCode.Add(keycodeExit);
      hookKeys.Callback += HookKeys_Callback;

      _timer = new System.Windows.Forms.Timer { Interval = 200, Enabled = false };
      _timer.Tick += OnTimerTicked;
    }

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

    public void BeginSpying()
    {
      _locator?.Close();
      _locator?.Dispose();
      _locator = new SpyAgentLocator();

      MakePassThrough(_locator.Handle);
      _timer.Enabled = true;
      hookKeys.SetupHook();
    }

    private void HookKeys_Callback(int keycode)
    {
      if (SpiedWindowSelected == null) return;
      if (keycode == keycodeExit) EndSpying();
      else if (keycode == keycodeSelect) SpiedWindowSelected(this, GetHoveredWindow());
    }

    public void EndSpying()
    {
      _timer.Enabled = false;
      hookKeys.UnHook();
      _locator?.Close();
      _locator?.Dispose();
      _locator = null;
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