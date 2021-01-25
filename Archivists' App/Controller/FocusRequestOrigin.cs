namespace SoundExplorers.Controller {
  /// <summary>
  ///   The origin of a request to focus a control, in this application, a grid.
  /// </summary>
  public enum FocusRequestOrigin {
    /// <summary>
    ///   Either a focus request has not been received or its origin has not yet been
    ///   specified.
    /// </summary>
    NotSpecified,
    
    /// <summary>
    ///   The origin of the focus request was a keyboard shortcut, in this application,
    ///   F6. 
    /// </summary>
    KeyboardShortcut,
    
    /// <summary>
    ///   The origin of the focus request was a mouse right-click.
    /// </summary>
    MouseRightClick,
    
    /// <summary>
    ///   The origin of the focus request was a windows message, which resulted from
    ///   either a mouse left-click or the application being brought into the foreground.
    /// </summary>
    WindowsMessage
  }
}