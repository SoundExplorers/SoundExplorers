namespace SoundExplorers.Controller {
  public interface ICellColorScheme {
    /// <summary>
    ///   Inverts the foreground and background colours of both selected and unselected
    ///   cells in the grid, relative to their defaults.
    /// </summary>
    void Invert();

    /// <summary>
    ///   Restores the foreground and background colours of both selected and unselected
    ///   cells in the grid to their defaults.
    /// </summary>
    void RestoreToDefault();
  }
}