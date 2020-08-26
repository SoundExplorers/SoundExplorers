namespace SoundExplorers.Data {
    /// <summary>
    ///   Role entity.
    /// </summary>
    internal class Role : Entity<Role> {
    [UniqueKeyField] public string Name { get; set; }
    [PrimaryKeyField] public string RoleId { get; set; }
  } //End of class
} //End of namespace