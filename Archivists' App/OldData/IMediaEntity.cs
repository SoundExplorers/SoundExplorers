namespace SoundExplorers.OldData {
  /// <summary>
  ///   Interface for
  ///   an Entity from whose data media file metadata tags may need to be updated.
  /// </summary>
  public interface IMediaEntity : IEntity {
    /// <summary>
    ///   Updates the metadata tags of any audio files
    ///   associated with the entity with appropriate data from the database.
    /// </summary>
    /// <returns>
    ///   A message describing the update or,
    ///   if no audio files have been updated,
    ///   an empty string.
    /// </returns>
    string UpdateTags();
  } //End of class
} //End of namespace