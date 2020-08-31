namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity column container interface.
  /// </summary>
  public interface IEntityColumnContainer {
    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's
    ///   field properties.
    /// </summary>
    EntityColumnList Columns { get; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the Entity's
    ///   field properties that are not in the primary key.
    /// </summary>
    EntityColumnList NonPrimaryKeyColumns { get; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the Entity's
    ///   field properties that are not in the unique key
    ///   (if there is one).
    /// </summary>
    EntityColumnList NonUniqueKeyColumns { get; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's
    ///   primary key field properties.
    /// </summary>
    EntityColumnList PrimaryKeyColumns { get; }

    /// <summary>
    ///   Gets the name of the database table
    ///   containing the row represented by the entity.
    /// </summary>
    string TableName { get; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's
    ///   unique key field properties.
    /// </summary>
    /// <remarks>
    ///   Empty if there is no unique key.
    /// </remarks>
    EntityColumnList UniqueKeyColumns { get; }
  } //End of class
} //End of namespace