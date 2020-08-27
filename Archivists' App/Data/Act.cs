namespace SoundExplorers.Data {
  /// <summary>
  ///   Act entity.
  /// </summary>
  internal class Act : PieceOwningMediaEntity<Act> {
    [Field] public string Comments { get; set; }
    [PrimaryKeyField] public string Name { get; set; }
  } //End of class
} //End of namespace