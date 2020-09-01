using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Credit entity.
  /// </summary>
  public class Credit : Entity<Credit>, IMediaEntity {
    [Field(0)] public string Act { get; set; }

    [PrimaryKeyField(1)]
    [ReferencedField("Name", 1)]
    public string Artist { get; set; }

    [PrimaryKeyField] public DateTime Date { get; set; }
    [HiddenField] public string Forename { get; set; }

    [PrimaryKeyField]
    [ReferencedField("Name")]
    public string Location { get; set; }

    [PrimaryKeyField]
    [ReferencedField("PieceNo")]
    public int Piece { get; set; }

    [PrimaryKeyField(2)]
    [ReferencedField("Name",2)]
    public string Role { get; set; }

    [PrimaryKeyField]
    [ReferencedField("SetNo")]
    public int Set { get; set; }

    [HiddenField] public string Surname { get; set; }

    /// <summary>
    ///   Updates the metadata tags of any audio file
    ///   associated with the Credit's Piece with appropriate data from the database.
    /// </summary>
    /// <returns>
    ///   A message describing the update or,
    ///   if no audio file has been updated,
    ///   an empty string.
    /// </returns>
    /// <exception cref="ApplicationException">
    ///   An error occured while attempting to save the tags.
    /// </exception>
    public string UpdateTags() {
      var piece = new Piece();
      piece.Date = Date;
      piece.Location = Location;
      piece.Set = Set;
      piece.PieceNo = Piece;
      piece.Fetch();
      return piece.UpdateTags();
    }
  } //End of class
} //End of namespace