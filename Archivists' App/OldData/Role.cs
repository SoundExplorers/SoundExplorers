namespace SoundExplorers.OldData {
  /// <summary>
  ///   Role entity.
  /// </summary>
  internal class Role : Entity<Role> {
    [UniqueKeyField(2)] public string Name { get; set; }
    [PrimaryKeyField(1)] public string RoleId { get; set; }
  } //End of class
} //End of namespace