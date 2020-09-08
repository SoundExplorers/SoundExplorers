namespace SoundExplorers.OldData {
  /// <summary>
  ///   ArtistInImage entity.
  /// </summary>
  public class ArtistInImage : Entity<ArtistInImage> {
    [PrimaryKeyField]
    [ReferencedField("Name")]
    public string Artist { get; set; }

    [HiddenField] public string Forename { get; set; }
    [PrimaryKeyField] public int ImageId { get; set; }
    [HiddenField] public string Surname { get; set; }
  } //End of class
} //End of namespace