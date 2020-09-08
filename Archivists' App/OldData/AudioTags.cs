namespace SoundExplorers.OldData {
  /// <summary>
  ///   The audio metadata tags
  ///   that we can usefully map to info on the database
  ///   and that are also available with all audio file types.
  /// </summary>
  internal class AudioTags : IAudioTags {
    /// <summary>
    ///   Gets or sets the Album audio metadata tag.
    /// </summary>
    public virtual string Album { get; set; }

    /// <summary>
    ///   Gets or sets the Artist audio metadata tag.
    /// </summary>
    public virtual string Artist { get; set; }

    /// <summary>
    ///   Gets or sets the Comment audio metadata tag.
    /// </summary>
    public virtual string Comment { get; set; }

    /// <summary>
    ///   Gets or sets the Title audio metadata tag.
    /// </summary>
    public virtual string Title { get; set; }

    /// <summary>
    ///   Gets or sets the Track audio metadata tag.
    /// </summary>
    public virtual uint Track { get; set; }

    /// <summary>
    ///   Gets or sets the Year audio metadata tag.
    /// </summary>
    public virtual uint Year { get; set; }
  } //End of class
} //End of namespace