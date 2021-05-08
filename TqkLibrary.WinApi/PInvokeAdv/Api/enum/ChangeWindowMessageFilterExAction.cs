namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
  /// <summary>
  /// values used by changewindowmessagefilterex
  /// </summary>
  public enum ChangeWindowMessageFilterExAction : uint
  {
    /// <summary>
    /// resets the window message filter for hwnd to the default.
    /// any message allowed globally or process-wide will get through,
    /// but any message not included in those two categories,
    /// and which comes from a lower privileged process, will be blocked.
    /// </summary>
    Reset = 0,

    /// <summary>
    /// allows the message through the filter.
    /// this enables the message to be received by hwnd,
    /// regardless of the source of the message,
    /// even it comes from a lower privileged process.
    /// </summary>
    Allow = 1,

    /// <summary>
    /// blocks the message to be delivered to hwnd if it comes from
    /// a lower privileged process, unless the message is allowed process-wide
    /// by using the changewindowmessagefilter function or globally.
    /// </summary>
    Disallow = 2
  }
}