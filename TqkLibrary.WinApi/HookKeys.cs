using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi
{
  public delegate void HookCallBack(int keycode);

  public class HookKeys
  {
    public List<int> KeyCode { get; } = new List<int>();

    public event HookCallBack Callback;

    private User32.WindowsHookDelegate windowsHook;
    private User32.SafeHookHandle WindowsHookExHandle;
    private IntPtr handle;

    public HookKeys()
    {
      windowsHook = HookCallback;//make that delegate not release by GC
    }

    public void SetupHook()
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        using (var ModuleHandle = Kernel32.GetModuleHandle(curModule.ModuleName))
        {
          WindowsHookExHandle = User32.SetWindowsHookEx(User32.WindowsHookType.WH_KEYBOARD_LL, windowsHook, ModuleHandle.DangerousGetHandle(), 0);
          handle = WindowsHookExHandle.DangerousGetHandle();
        }
      }
    }

    public void UnHook()
    {
      if (WindowsHookExHandle != null &&
         !WindowsHookExHandle.IsClosed &&
         !WindowsHookExHandle.IsInvalid)
        WindowsHookExHandle.Dispose();

      WindowsHookExHandle = null;
    }

    private int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      if (nCode >= 0 && wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN)
      {
        int vkCode = Marshal.ReadInt32(lParam);
        if (KeyCode.Count > 0)
        {
          if (KeyCode.Any(x => x == vkCode)) Callback?.Invoke(vkCode);
        }
        else Callback?.Invoke(vkCode);
      }
      return User32.CallNextHookEx(handle, nCode, wParam, lParam);
    }
  }
}