namespace SoundExplorers.Data {
  /// <summary>
  ///   Act entity.
  /// </summary>
  internal class Act : PieceOwningMediaEntity<Act> {
    [Field(2)] public string Comments { get; set; }
    [PrimaryKeyField(1)] public string Name { get; set; }
  } //End of class
} //End of namespace