using Microsoft.Win32;
using System.Linq;

namespace TqkLibrary.WinApi
{
  public static class RegisterSchemeUri
  {
    /// <summary>
    ///
    /// </summary>
    /// <param name="scheme"></param>
    /// <param name="exePath"></param>
    /// <returns>false if exist, true when success</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    /// <exception cref="System.ObjectDisposedException"></exception>
    /// <exception cref="System.UnauthorizedAccessException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    public static bool Register(string scheme, string exePath)
    {
      if (Registry.ClassesRoot.GetSubKeyNames().Any(x => x.Equals(scheme))) return false;
      else
      {
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(scheme))
        {
          using (var key_shell = key.CreateSubKey("shell"))
          {
            key.SetValue("", "URL: Foo Protocol");
            key.SetValue("URL Protocol", "");
            using (var key_open = key_shell.CreateSubKey("open"))
            using (var key_command = key_open.CreateSubKey("command")) key_command.SetValue("", exePath);
          }
        }
      }
      return true;
    }

    public static bool UnRegister(string scheme)
    {
      if (Registry.ClassesRoot.GetSubKeyNames().Where(x => x.Equals(scheme)).Count() > 0)
      {
        Registry.ClassesRoot.DeleteSubKeyTree(scheme);
        return true;
      }
      else return false;
    }
  }
}