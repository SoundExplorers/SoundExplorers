using TagLib;

namespace SoundExplorers.Data {
    /// <summary>
    ///   The values actually or intended to be
    ///   held in an audio file of
    ///   those audio metadata tags
    ///   that we can usefully map to info on the database
    ///   and that are also available with all audio file types.
    /// </summary>
    internal class AudioFileTags : IAudioTags {
        /// <summary>
        ///   Initialises a new instance of the
        ///   <see cref="AudioFileTags" /> class.
        /// </summary>
        /// <param name="tags">
        ///   The audio file's metadata tags.
        /// </param>
        public AudioFileTags(Tag tags) {
      Tags = tags;
    }

    private Tag Tags { get; }

    /// <summary>
    ///   Gets or sets the audio file's Album metadata tag.
    /// </summary>
    public string Album {
      get => Tags.Album;
      set => Tags.Album = value;
    }

    /// <summary>
    ///   Gets or sets the audio file's Artist metadata tag.
    /// </summary>
    public string Artist {
      get => Tags.FirstPerformer;
      set { Tags.Performers = new[] {value}; }
    }

    /// <summary>
    ///   Gets or sets the audio file's Comment metadata tag.
    /// </summary>
    public string Comment {
      get => Tags.Comment;
      set => Tags.Comment = value;
    }

    /// <summary>
    ///   Gets or sets the audio file's Title metadata tag.
    /// </summary>
    public string Title {
      get => Tags.Title;
      set => Tags.Title = value;
    }

    /// <summary>
    ///   Gets or sets the audio file's Track metadata tag.
    /// </summary>
    public uint Track {
      get => Tags.Track;
      set => Tags.Track = value;
    }

    /// <summary>
    ///   Gets or sets the audio file's Year metadata tag.
    /// </summary>
    public uint Year {
      get => Tags.Year;
      set => Tags.Year = value;
    }
  } //End of class
} //End of namespace