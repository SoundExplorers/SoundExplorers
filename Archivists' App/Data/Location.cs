namespace SoundExplorers.Data {
    /// <summary>
    ///   Location entity.
    /// </summary>
    internal class Location : PieceOwningMediaEntity<Location> {
    [Field] public string Comments { get; set; }
    [PrimaryKeyField] public string LocationId { get; set; }
    [UniqueKeyField] public string Name { get; set; }
  } //End of class
} //End of namespace