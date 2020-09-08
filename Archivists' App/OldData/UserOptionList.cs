namespace SoundExplorers.OldData {
  /// <summary>
  ///   A list of UserOptions.
  /// </summary>
  internal class UserOptionList : EntityList<UserOption> {
    /// <summary>
    ///   An indexer that returns
    ///   the UserOption at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the UserOption to get.
    /// </param>
    /// <returns>
    ///   The UserOption at the specified index in the list.
    /// </returns>
    public new UserOption this[int index] => base[index] as UserOption;
  } //End of class
} //End of namespace