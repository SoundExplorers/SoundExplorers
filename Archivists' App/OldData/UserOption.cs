namespace SoundExplorers.OldData {
  /// <summary>
  ///   UserOption entity.
  /// </summary>
  internal class UserOption : Entity<UserOption> {
    [PrimaryKeyField(2)] public string OptionName { get; set; }
    [Field(3)] public string OptionValue { get; set; }
    [PrimaryKeyField(1)] public string UserId { get; set; }
  } //End of class
} //End of namespace