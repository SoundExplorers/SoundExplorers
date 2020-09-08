using System;
using System.IO;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   An Entity that can have mutliple Pieces associated with it
  ///   and from whose data media file metadata tags may need to be updated.
  /// </summary>
  public abstract class PieceOwningMediaEntity<T> : PieceOwningEntity<T>,
    IMediaEntity
    where T : PieceOwningMediaEntity<T> {
    /// <summary>
    ///   Updates the metadata tags of any audio files
    ///   associated with the entity with appropriate data from the database.
    /// </summary>
    /// <returns>
    ///   A message describing the update or,
    ///   if no audio files have been updated,
    ///   an empty string.
    /// </returns>
    /// <exception cref="ApplicationException">
    ///   An error occured while attempting to save the tags.
    /// </exception>
    public string UpdateTags() {
      var count = 0;
      Pieces = FetchPieces();
      foreach (Piece piece in Pieces) {
        if (piece.AudioTags.SaveToFile()) {
          count++;
        }
      }
      switch (count) {
        case 0:
          return string.Empty;
        case 1:
          return
            "Tags saved to audio file \""
            + Path.GetFileName(Pieces[0].AudioPath) + "\"";
        default:
          return
            "Tags saved to " + count.ToString("#,0") + " audio files.";
      } //End of switch
    }
  } //End of class
} //End of namespace