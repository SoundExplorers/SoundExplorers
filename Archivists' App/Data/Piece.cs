using System;
using System.IO;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Piece entity.
  /// </summary>
  internal class Piece : Entity<Piece>, IMediaEntity {
    private static DirectoryInfo _defaultAudioFolder;
    private static Option _defaultAudioFolderOption;
    private static DirectoryInfo _defaultVideoFolder;
    private static Option _defaultVideoFolderOption;
    private CreditList _credits;
    private Piece _original;

    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="Piece" /> class.
    /// </summary>
    public Piece() {
      AudioTags = new PieceAudioTags(this);
    }

    [ReferencedField("Name")] public string Act { get; set; }

    /// <summary>
    ///   Gets whether the file, if any, specified by
    ///   the <see cref="AudioPath" /> field propery exists.
    ///   False if a path is not specified or the file does not exist.
    /// </summary>
    public virtual bool AudioFileExists =>
      !string.IsNullOrEmpty(AudioPath)
      && File.Exists(AudioPath);

    [Field] public string AudioPath { get; set; }

    /// <summary>
    ///   Gets the values generated from the data of the Piece and its Credits
    ///   for those audio metadata tags
    ///   that are available all audio file types
    ///   and may or may not be saved to an actual audio file.
    /// </summary>
    public PieceAudioTags AudioTags { get; }

    [Field] public string Comments { get; set; }

    /// <summary>
    ///   Gets or sets the Piece's child Credits.
    ///   Will be fetched from the database if not previously set when got.
    ///   Will be sorted into Surname, Forename, Role order.
    /// </summary>
    public virtual CreditList Credits {
      get {
        if (_credits == null) {
          _credits = FetchCredits();
        }
        return _credits;
      }
      set => _credits = value;
    }

    [PrimaryKeyField] public DateTime Date { get; set; }

    /// <summary>
    ///   Gets or sets a default folder to be used,
    ///   if the folder of the path (if any) currently in
    ///   the <see cref="AudioPath" /> property
    ///   is not specified or does not exist,
    ///   as the initial folder for the Open dialogue that may be
    ///   shown to select an audio file to update the path in
    ///   the <see cref="AudioPath" /> property.
    /// </summary>
    public static DirectoryInfo DefaultAudioFolder {
      get {
        if (_defaultAudioFolder == null) {
          if (DefaultAudioFolderOption.StringValue != string.Empty) {
            _defaultAudioFolder =
              new DirectoryInfo(DefaultAudioFolderOption.StringValue);
          }
          if (_defaultAudioFolder == null
              || !_defaultAudioFolder.Exists) {
            _defaultAudioFolder = new DirectoryInfo(
              Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
          }
        }
        return _defaultAudioFolder;
      }
      set {
        _defaultAudioFolder = value;
        DefaultAudioFolderOption.StringValue = value.FullName;
      }
    }

    private static Option DefaultAudioFolderOption {
      get {
        if (_defaultAudioFolderOption == null) {
          _defaultAudioFolderOption = new Option(
            "Piece.DefaultAudioFolder");
        }
        return _defaultAudioFolderOption;
      }
    }

    /// <summary>
    ///   Gets or sets a default folder to be used,
    ///   if the folder of the path (if any) currently in
    ///   the <see cref="VideoPath" /> property
    ///   is not specified or does not exist,
    ///   as the initial folder for the Open dialogue that may be
    ///   shown to select a video file to update the path in
    ///   the <see cref="VideoPath" /> property.
    /// </summary>
    public static DirectoryInfo DefaultVideoFolder {
      get {
        if (_defaultVideoFolder == null) {
          if (DefaultVideoFolderOption.StringValue != string.Empty) {
            _defaultVideoFolder =
              new DirectoryInfo(DefaultVideoFolderOption.StringValue);
          }
          if (_defaultVideoFolder == null
              || !_defaultVideoFolder.Exists) {
            _defaultVideoFolder = new DirectoryInfo(
              Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
          }
        }
        return _defaultVideoFolder;
      }
      set {
        _defaultVideoFolder = value;
        DefaultVideoFolderOption.StringValue = value.FullName;
      }
    }

    private static Option DefaultVideoFolderOption {
      get {
        if (_defaultVideoFolderOption == null) {
          _defaultVideoFolderOption = new Option(
            "Piece.DefaultVideoFolder");
        }
        return _defaultVideoFolderOption;
      }
    }

    [PrimaryKeyField]
    [ReferencedField("Name")]
    public string Location { get; set; }

    [ReferencedField("Date")] public DateTime Newsletter { get; set; }

    /// <summary>
    ///   Gets or sets a version of the Piece and its child Credits
    ///   with field property values as at
    ///   when the corresponding grid row was entered,
    ///   i.e. as at before any changed effected through the grid.
    /// </summary>
    public virtual Piece Original {
      get {
        if (_original == null) {
          _original = this;
        }
        return _original;
      }
      set => _original = value;
    }

    [PrimaryKeyField] public int PieceNo { get; set; }

    [PrimaryKeyField]
    [ReferencedField("SetNo")]
    public int Set { get; set; }

    [Field] public string Title { get; set; }

    /// <summary>
    ///   Gets whether the file, if any, specified by
    ///   the <see cref="VideoPath" /> field propery exists.
    ///   False if a path is not specified or the file does not exist.
    /// </summary>
    public virtual bool VideoFileExists =>
      !string.IsNullOrEmpty(VideoPath)
      && File.Exists(VideoPath);

    [Field] public string VideoPath { get; set; }

    /// <summary>
    ///   Updates the metadata tags of any audio file
    ///   associated with the Piece with appropriate data from the database.
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
      if (AudioTags.SaveToFile()) {
        return
          "Tags saved to audio file \""
          + Path.GetFileName(AudioPath) + "\"";
      }
      return string.Empty;
    }

    /// <summary>
    ///   Fetches the Piece's child Credits from the database.
    /// </summary>
    /// <returns>
    ///   The Piece's child Credits.
    /// </returns>
    public virtual CreditList FetchCredits() {
      return new CreditList(
        null, this);
    }

    /// <summary>
    ///   Fetches the Piece's Newsletter (i.e. not just the Newsletter date)
    ///   from the database.
    /// </summary>
    /// <returns>
    ///   The Piece's Newsletter.
    /// </returns>
    public virtual Newsletter FetchNewsletter() {
      var newsletter = new Newsletter();
      newsletter.Date = Newsletter;
      newsletter.Fetch();
      return newsletter;
    }
  } //End of class
} //End of namespace