namespace SoundExplorers.Data {
  /// <summary>
  ///   Location entity.
  /// </summary>
  internal class Location : PieceOwningMediaEntity<Location> {
    [Field(3)] public string Comments { get; set; }
    [PrimaryKeyField(1)] public string LocationId { get; set; }
    [UniqueKeyField(2)] public string Name { get; set; }
  } //End of class
} //End of namespace