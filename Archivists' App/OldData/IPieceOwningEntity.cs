namespace SoundExplorers.OldData {
  /// <summary>
  ///   Interface for
  ///   an Entity that can have mutliple Pieces associated with it.
  /// </summary>
  public interface IPieceOwningEntity : IEntity {
    /// <summary>
    ///   Gets or sets the entity's Pieces.
    ///   Will be fetched from the database if not previously set when got.
    /// </summary>
    PieceList Pieces { get; set; }

    /// <summary>
    ///   Fetches the entity's Pieces from the database.
    /// </summary>
    /// <returns>
    ///   The entitye's Pieces.
    /// </returns>
    PieceList FetchPieces();
  } //End of class
} //End of namespace