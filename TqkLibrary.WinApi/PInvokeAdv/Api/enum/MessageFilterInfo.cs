namespace TqkLibrary.WinApi.PInvokeAdv.Api
{
  /// <summary>
  /// Values used in the struct CHANGEFILTERSTRUCT
  /// </summary>
  public enum MessageFilterInfo : uint
  {
    /// <summary>
    /// Certain messages whose value is smaller than WM_USER are required to pass
    /// through the filter, regardless of the filter setting.
    /// There will be no effect when you attempt to use this function to
    /// allow or block such messages.
    /// </summary>
    None = 0,

    /// <summary>
    /// The message has already been allowed by this window's message filter,
    /// and the function thus succeeded with no change to the window's message filter.
    /// Applies to MSGFLT_ALLOW.
    /// </summary>
    AlreadyAllowed = 1,

    /// <summary>
    /// The message has already been blocked by this window's message filter,
    /// and the function thus succeeded with no change to the window's message filter.
    /// Applies to MSGFLT_DISALLOW.
    /// </summary>
    AlreadyDisAllowed = 2,

    /// <summary>
    /// The message is allowed at a scope higher than the window.
    /// Applies to MSGFLT_DISALLOW.
    /// </summary>
    AllowedHigher = 3
  }
}