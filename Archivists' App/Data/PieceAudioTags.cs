using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SoundExplorers.Data {
    /// <summary>
    ///   The values generated from the data of a <see cref="Piece" />
    ///   and its <see cref="Piece.Credits" />
    ///   for those audio metadata tags
    ///   that are available all audio file types
    ///   and may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   To save all the generated metadata tags to the audio file,
    ///   if it exists,
    ///   specified by the <see cref="Piece" />'s
    ///   <see cref="Piece.AudioPath" /> property,
    ///   invoke the <see cref="SaveToFile" /> method.
    /// </remarks>
    internal class PieceAudioTags : IAudioTags {
        /// <summary>
        ///   Initialises a new instance of the
        ///   <see cref="PieceAudioTags" /> class.
        /// </summary>
        /// <param name="piece">
        ///   The Piece from whose data the tags are to be generated.
        /// </param>
        public PieceAudioTags(Piece piece) {
      Piece = piece;
    }

    private Piece Piece { get; }

    /// <summary>
    ///   Gets or sets the generated Album audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   <see cref="Piece.Date" />, <see cref="Piece.Location" />
    ///   and <see cref="Piece.Set" /> in this format:
    ///   yyyy/MM/dd at LOCATION (set SET).
    ///   For example:
    ///   2010/12/31 at Fred's (set 2)
    /// </remarks>
    public string Album =>
      Piece.Date.ToString("yyyy/MM/dd")
      + " at " + Piece.Location + " (set " + Piece.Set + ")";

    /// <summary>
    ///   Gets or sets the generated Artist audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   If the Piece's Act has been specified,
    ///   the Artist tag will be the Act name.
    ///   For example:
    ///   Orchestra of Spheres
    ///   Otherwise if the Piece has a single Artist,
    ///   the Artist tag will be the Artist's name.
    ///   For example:
    ///   Daniel Beban
    ///   Otherwise if the Piece has a multiple Artists,
    ///   the Artist tag will be the Artist surnames
    ///   (or forenames where surnames are not specified)
    ///   in alphabetical order and delimited by /.
    ///   For example:
    ///   Beban/O'Rorke/Prosser
    ///   Otherwise (neither Act nor Credits specified for the Piece),
    ///   the Artist tag will be an empty string.
    /// </remarks>
    public string Artist {
      get {
        if (Piece.Act != string.Empty) {
          return Piece.Act;
        }
        if (Piece.Credits.Count == 0) {
          return string.Empty;
        }
        // Check we don't use the same artist name twice,
        // i.e. in the event of multiple roles for one artist. 
        var artistNames = new List<string>(Piece.Credits.Count);
        var artist = new StringWriter();
        for (var i = 0; i < Piece.Credits.Count; i++) {
          if (!artistNames.Contains(Piece.Credits[i].Artist)) {
            artistNames.Add(Piece.Credits[i].Artist);
            if (!string.IsNullOrWhiteSpace(Piece.Credits[i].Surname)) {
              artist.Write(Piece.Credits[i].Surname);
            } else {
              artist.Write(Piece.Credits[i].Forename);
            }
            if (i < Piece.Credits.Count - 1) {
              artist.Write("/");
            }
          }
        } //End of for
        if (artistNames.Count > 1) {
          return artist.ToString();
        }
        return artistNames[0];
      }
    }

    /// <summary>
    ///   Gets or sets the generated Comment audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   <see cref="Piece.Credits" />, if any,
    ///   in Surname, Forename, Role order
    ///   followed by
    ///   Recorded on <see cref="Piece.Date" /> at <see cref="Piece.Location" />
    ///   followed by <see cref="Piece.Comments" />, if any.
    ///   For example:
    ///   Brian Artichoke - guitar
    ///   Zara Artichoke - bass guitar, cello, drums, euphonium, fiddle, zither
    ///   Andrea Zagrebski - accordion
    ///   Recorded on 01 December 2008 at Fred's.
    ///   Comments line 1
    ///   Comments line 2
    /// </remarks>
    public string Comment {
      get {
        var comment = new StringWriter();
        if (Piece.Credits.Count > 0) {
          comment.Write(Piece.Credits[0].Artist + " - " +
                        Piece.Credits[0].Role);
          string lastArtist = Piece.Credits[0].Artist;
          if (Piece.Credits.Count > 1) {
            for (var i = 1; i < Piece.Credits.Count; i++) {
              var credit = Piece.Credits[i];
              if (credit.Artist == lastArtist) {
                comment.Write(", " + credit.Role);
              } else {
                comment.WriteLine();
                comment.Write(credit.Artist + " - " + credit.Role);
                lastArtist = credit.Artist;
              }
            } //End of for
          }
          comment.WriteLine();
        }
        comment.Write(
          "Recorded on " + Piece.Date.ToString("dd MMMM yyyy")
                         + " at " + Piece.Location + ".");
        if (Piece.Comments != string.Empty) {
          comment.WriteLine();
          comment.WriteLine();
          comment.Write(Piece.Comments);
        }
        return comment.ToString();
      }
    }

    /// <summary>
    ///   Gets or sets the generated Title audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   <see cref="Piece.Title" />.
    /// </remarks>
    public string Title => Piece.Title;

    /// <summary>
    ///   Gets or sets the generated Track audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   <see cref="Piece.PieceNo" />.
    /// </remarks>
    public uint Track => (uint)Piece.PieceNo;

    /// <summary>
    ///   Gets or sets the generated Year audio metadata tag,
    ///   which may or may not be saved to an actual audio file.
    /// </summary>
    /// <remarks>
    ///   Year of <see cref="Piece.Date" />.
    /// </remarks>
    public uint Year => (uint)Piece.Date.Year;

    /// <summary>
    ///   Saves all the generated metadata tags to the audio file,
    ///   if it exists,
    ///   specified by the <see cref="Piece" />'s
    ///   <see cref="Piece.AudioPath" /> property.
    /// </summary>
    /// <param name="force">
    ///   Whether each current generated tag should forced to be saved to the file
    ///   even where the corresponding file tag specifies something
    ///   different from the original generated tag.
    ///   Default True for now.
    /// </param>
    /// <returns>
    ///   Whether the audio file exists
    ///   and changes were required to its tags,
    ///   which were therefore saved to the file.
    /// </returns>
    /// <exception cref="ApplicationException">
    ///   An error occured while attempting to save the tags.
    /// </exception>
    /// <remarks>
    ///   Unless the <paramref name="force" /> parameter is true,
    ///   to avoid overriding any of the file's tags that may
    ///   have been manually edited,
    ///   with one exception
    ///   no tag will be updated unless its initial
    ///   value is either unspecified or
    ///   as it would have been generated
    ///   when the corresponding grid row was entered,
    ///   i.e. as at before any changed effected through the grid.
    ///   ???
    ///   The exception is Title, where the generated Title will always
    ///   be used if generated because file Titles are usually generated
    ///   from the file name if unspecified.
    ///   The file won't actually get updated if nothing has changed.
    /// </remarks>
    public bool SaveToFile(bool force = true) {
      if (!Piece.AudioFileExists) {
        return false;
      }
      Debug.WriteLine("Original Title = " + Piece.Original.AudioTags.Title);
      Debug.WriteLine("Original Artist = " + Piece.Original.AudioTags.Artist);
      Debug.WriteLine("Original Album = " + Piece.Original.AudioTags.Album);
      Debug.WriteLine("Original Track = " + Piece.Original.AudioTags.Track);
      Debug.WriteLine("Original Year = " + Piece.Original.AudioTags.Year);
      Debug.WriteLine("Original Comment = " + Piece.Original.AudioTags.Comment);
      Debug.WriteLine("Piece Title = " + Piece.AudioTags.Title);
      Debug.WriteLine("Piece Artist = " + Piece.AudioTags.Artist);
      Debug.WriteLine("Piece Album = " + Piece.AudioTags.Album);
      Debug.WriteLine("Piece Track = " + Piece.AudioTags.Track);
      Debug.WriteLine("Piece Year = " + Piece.AudioTags.Year);
      Debug.WriteLine("Piece Comment = " + Piece.AudioTags.Comment);
      var audioFile = new AudioFile(Piece.AudioPath);
      try {
        Debug.WriteLine("File Title = " + audioFile.Tags.Title);
        Debug.WriteLine("File Artist = " + audioFile.Tags.Artist);
        Debug.WriteLine("File Album = " + audioFile.Tags.Album);
        Debug.WriteLine("File Track = " + audioFile.Tags.Track);
        Debug.WriteLine("File Year = " + audioFile.Tags.Year);
        Debug.WriteLine("File Comment = " + audioFile.Tags.Comment);
        if (force) {
          audioFile.Tags.Title = Piece.AudioTags.Title;
          audioFile.Tags.Artist = Piece.AudioTags.Artist;
          audioFile.Tags.Album = Piece.AudioTags.Album;
          audioFile.Tags.Track = Piece.AudioTags.Track;
          audioFile.Tags.Year = Piece.AudioTags.Year;
          audioFile.Tags.Comment = Piece.AudioTags.Comment;
        } else {
          if (string.IsNullOrEmpty(audioFile.Tags.Title)
              || audioFile.Tags.Title == Piece.Original.AudioTags.Title
              || Piece.AudioTags.Title != string.Empty) {
            audioFile.Tags.Title = Piece.AudioTags.Title;
          }
          if (string.IsNullOrEmpty(audioFile.Tags.Artist)
              || audioFile.Tags.Artist == Piece.Original.AudioTags.Artist) {
            audioFile.Tags.Artist = Piece.AudioTags.Artist;
          }
          if (string.IsNullOrEmpty(audioFile.Tags.Album)
              || audioFile.Tags.Album == Piece.Original.AudioTags.Album) {
            audioFile.Tags.Album = Piece.AudioTags.Album;
          }
          if (audioFile.Tags.Track == 0
              || audioFile.Tags.Track == Piece.Original.AudioTags.Track) {
            audioFile.Tags.Track = Piece.AudioTags.Track;
          }
          if (audioFile.Tags.Year == 0
              || audioFile.Tags.Year == Piece.Original.AudioTags.Year) {
            audioFile.Tags.Year = Piece.AudioTags.Year;
          }
          if (string.IsNullOrEmpty(audioFile.Tags.Comment)
              || audioFile.Tags.Comment == Piece.Original.AudioTags.Comment) {
            audioFile.Tags.Comment = Piece.AudioTags.Comment;
          }
        }
        Debug.WriteLine("Updated Title = " + audioFile.Tags.Title);
        Debug.WriteLine("Updated Artist = " + audioFile.Tags.Artist);
        Debug.WriteLine("Updated Album = " + audioFile.Tags.Album);
        Debug.WriteLine("Updated Track = " + audioFile.Tags.Track);
        Debug.WriteLine("Updated Year = " + audioFile.Tags.Year);
        Debug.WriteLine("Updated Comment = " + audioFile.Tags.Comment);
        // The file won't actually get updated if nothing has changed.
        return audioFile.SaveTags();
      } finally {
        audioFile.Dispose();
      }
    }
  } //End of class
} //End of namespace