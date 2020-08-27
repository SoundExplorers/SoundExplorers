namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Roles.
  /// </summary>
  internal class RoleList : EntityList<Role> {
    /// <summary>
    ///   An indexer that returns
    ///   the Role at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Role to get.
    /// </param>
    /// <returns>
    ///   The Role at the specified index in the list.
    /// </returns>
    public new Role this[int index] => base[index] as Role;
  } //End of class
} //End of namespace