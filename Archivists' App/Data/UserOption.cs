namespace SoundExplorers.Data {
  /// <summary>
  ///   UserOption entity.
  /// </summary>
  internal class UserOption : Entity<UserOption> {
    [PrimaryKeyField] public string OptionName { get; set; }
    [Field] public string OptionValue { get; set; }
    [PrimaryKeyField] public string UserId { get; set; }
  } //End of class
} //End of namespace