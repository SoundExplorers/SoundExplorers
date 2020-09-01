namespace SoundExplorers.Data {
  /// <summary>
  ///   An Entity that can have mutliple Pieces associated with it.
  /// </summary>
  public abstract class PieceOwningEntity<T> : Entity<T>, IPieceOwningEntity
    where T : PieceOwningEntity<T> {
    private PieceList _pieces;

    /// <summary>
    ///   Fetches the entity's Pieces from the database.
    /// </summary>
    /// <returns>
    ///   The entity's Pieces.
    /// </returns>
    //public abstract PieceList FetchPieces();
    public PieceList FetchPieces() {
      return new PieceList(
        null, this);
    }

    /// <summary>
    ///   Gets or sets the entity's Pieces.
    ///   Will be fetched from the database if not previously set when got.
    /// </summary>
    public virtual PieceList Pieces {
      get {
        if (_pieces == null) {
          _pieces = FetchPieces();
        }
        return _pieces;
      }
      set => _pieces = value;
    }
  } //End of class
} //End of namespace