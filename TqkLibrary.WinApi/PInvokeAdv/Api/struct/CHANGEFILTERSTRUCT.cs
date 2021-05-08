using System.Runtime.InteropServices;

namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
  /// <summary>
  /// Contains extended result information obtained by calling
  /// the ChangeWindowMessageFilterEx function.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct CHANGEFILTERSTRUCT
  {
    /// <summary>
    /// The size of the structure, in bytes. Must be set to sizeof(CHANGEFILTERSTRUCT),
    /// otherwise the function fails with ERROR_INVALID_PARAMETER.
    /// </summary>
    public uint size;

    /// <summary>
    /// If the function succeeds, this field contains one of the following values,
    /// <see cref="MessageFilterInfo"/>
    /// </summary>
    public MessageFilterInfo info;
  }
}