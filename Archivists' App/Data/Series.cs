namespace SoundExplorers.Data {
  /// <summary>
  ///   Series entity.
  /// </summary>
  internal class Series : PieceOwningEntity<Series> {
    [Field(3)] public string Comments { get; set; }
    [UniqueKeyField(2)] public string Name { get; set; }
    [PrimaryKeyField(1)] public string SeriesId { get; set; }
  } //End of class
} //End of namespace