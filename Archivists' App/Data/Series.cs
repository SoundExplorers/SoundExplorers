namespace SoundExplorers.Data {
    /// <summary>
    ///   Series entity.
    /// </summary>
    internal class Series : PieceOwningEntity<Series> {
    [Field] public string Comments { get; set; }
    [UniqueKeyField] public string Name { get; set; }
    [PrimaryKeyField] public string SeriesId { get; set; }
  } //End of class
} //End of namespace