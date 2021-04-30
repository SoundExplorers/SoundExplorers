namespace SoundExplorers.View {
  public interface ICanRestoreErrorValue {
    /// <summary>
    ///   Restores the specified error value to the cell.
    ///   This will allow the user to correct the error value or,
    ///   by cancelling the edit, restore the original value.
    /// </summary>
    void RestoreErrorValue(object errorValue);
  }
}