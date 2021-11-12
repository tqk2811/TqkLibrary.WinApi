using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi
{
  public delegate void HookCallBack(int keycode,bool isDown);

  public class HookKeys
  {
    public bool HookAll { get; set; } = false;
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
        IntPtr ModuleHandle = Kernel32.GetModuleHandle(curModule.ModuleName);
        if (ModuleHandle != IntPtr.Zero)
        {
          WindowsHookExHandle = User32.SetWindowsHookEx(User32.WindowsHookType.WH_KEYBOARD_LL, windowsHook, ModuleHandle, 0);
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
      if(nCode >= 0 && (
        wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN ||
        wParam == (IntPtr)User32.WindowMessage.WM_KEYUP))
      {
        bool keyDown = wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN;
        int vkCode = Marshal.ReadInt32(lParam);
        if (HookAll) Callback?.Invoke(vkCode, keyDown);
        else if (KeyCode.Any(x => x == vkCode)) Callback?.Invoke(vkCode, keyDown);
      }
      return User32.CallNextHookEx(handle, nCode, wParam, lParam);
    }
  }
}