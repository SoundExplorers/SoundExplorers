using System;
using System.Diagnostics.CodeAnalysis;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a piece for which an audio and/or video
  ///   recording is archived.
  /// </summary>
  public class Piece : EntityBase {
    public const string DurationErrorMessage =
      "Duration must be between 1 second and 9 hours, 59 minutes, 59 seconds.";

    private string _audioUrl = null!;
    private TimeSpan _duration;
    private string _notes = null!;
    private int _pieceNo;
    private string _title = null!;
    private string _videoUrl = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Piece(SortedEntityCollection<Piece> root) : base(
      root,typeof(Piece), nameof(PieceNo), typeof(Set)) {
      Credits = new SortedEntityCollection<Credit>();
    }

    /// <summary>
    ///   The URL where the audio recording, if available, of the piece is archived.
    ///   Must be unique if specified.
    /// </summary>
    public string AudioUrl {
      get => _audioUrl;
      set {
        ValidateAudioUrl(_audioUrl, value);
        UpdateNonIndexField();
        _audioUrl = value;
      }
    }

    public SortedEntityCollection<Credit> Credits { get; }

    public TimeSpan Duration {
      get => _duration;
      set {
        ValidateDurationRange(value);
        UpdateNonIndexField();
        _duration = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public int PieceNo {
      get => _pieceNo;
      set {
        UpdateNonIndexField();
        _pieceNo = value;
        SimpleKey = IntegerToSimpleKey(value, nameof(PieceNo));
      }
    }

    public Set Set {
      get => (Set)IdentifyingParent!;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public string Title {
      get => _title;
      set {
        UpdateNonIndexField();
        _title = value;
      }
    }

    /// <summary>
    ///   The URL where the video recording, if available, of the piece is archived.
    ///   Must be unique if specified.
    /// </summary>
    public string VideoUrl {
      get => _videoUrl;
      set {
        ValidateVideoUrl(_videoUrl, value);
        UpdateNonIndexField();
        _videoUrl = value;
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      Piece? duplicate;
      if (!string.IsNullOrWhiteSpace(AudioUrl)) {
        duplicate = FindDuplicateAudioUrl(AudioUrl, session);
        if (duplicate != null) {
          throw CreateDuplicateAudioUrlInsertionException(Key, duplicate);
        }
      }
      if (!string.IsNullOrWhiteSpace(VideoUrl)) {
        duplicate = FindDuplicateVideoUrl(VideoUrl, session);
        if (duplicate != null) {
          throw CreateDuplicateVideoUrlInsertException(Key, duplicate);
        }
      }
      CheckThatDurationHasBeenSpecified(Key, Duration);
    }

    public static void CheckThatDurationHasBeenSpecified(Key key, TimeSpan duration) {
      if (duration == TimeSpan.Zero) {
        throw new PropertyConstraintException(
          $"Piece '{key}' cannot be added because its Duration has not been specified.",
          nameof(Duration));
      }
    }

    public static PropertyConstraintException CreateDuplicateAudioUrlInsertionException(
      Key newKey, Piece duplicate) {
      return new PropertyConstraintException(
        $"Piece '{newKey}' cannot be added because Piece " +
        $"'{duplicate.Key}' " +
        $"already exists with the same Audio URL '{duplicate.AudioUrl}'.",
        nameof(AudioUrl));
    }

    public static PropertyConstraintException CreateDuplicateAudioUrlUpdateException(
      Piece duplicate) {
      return new PropertyConstraintException(
        "Audio URL cannot be set to " +
        $"'{duplicate.AudioUrl}'. Piece '{duplicate.Key}' " +
        "already exists with that Audio URL.", nameof(AudioUrl));
    }

    public static PropertyConstraintException CreateDuplicateVideoUrlInsertException(
      Key newKey, Piece duplicate) {
      return new PropertyConstraintException(
        $"Piece '{newKey}' cannot be added because Piece " +
        $"'{duplicate.Key}' " +
        $"already exists with the same Video URL '{duplicate.VideoUrl}'.",
        nameof(VideoUrl));
    }

    public static PropertyConstraintException CreateDuplicateVideoUrlUpdateException(
      Piece duplicate) {
      return new PropertyConstraintException(
        "Video URL cannot be set to " +
        $"'{duplicate.VideoUrl}'. Piece '{duplicate.Key}' " +
        "already exists with that Video URL.", nameof(VideoUrl));
    }

    private Piece? FindDuplicateAudioUrl(string audioUrl,
      SessionBase session) {
      return QueryHelper.Find<Piece>(
        piece => !string.IsNullOrWhiteSpace(piece.AudioUrl) &&
                 piece.AudioUrl.Equals(audioUrl) &&
                 !piece.Oid.Equals(Oid),
        session);
    }

    private Piece? FindDuplicateVideoUrl(string videoUrl,
      SessionBase session) {
      return QueryHelper.Find<Piece>(
        piece => !string.IsNullOrWhiteSpace(piece.VideoUrl) &&
                 piece.VideoUrl.Equals(videoUrl) &&
                 !piece.Oid.Equals(Oid),
        session);
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Credits;
    }

    private void ValidateAudioUrl(string? oldAudioUrl,
      string? newAudioUrl) {
      if (string.IsNullOrWhiteSpace(newAudioUrl)) {
        return;
      }
      ValidateAudioUrlFormat(newAudioUrl);
      if (IsPersistent && Session != null && newAudioUrl != oldAudioUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw an InvalidOperationException anyway.
        var duplicate = FindDuplicateAudioUrl(newAudioUrl, Session);
        if (duplicate != null) {
          throw CreateDuplicateAudioUrlUpdateException(duplicate);
        }
      }
    }

    public static void ValidateAudioUrlFormat(string audioUrl) {
      ValidateUrlFormat(audioUrl, nameof(AudioUrl));
    }

    public static void ValidateDurationRange(TimeSpan value) {
      if (value < TimeSpan.FromSeconds(1) || value >= TimeSpan.FromHours(10)) {
        throw new PropertyConstraintException(
          DurationErrorMessage,
          nameof(Duration));
      }
    }

    private void ValidateVideoUrl(string? oldVideoUrl,
      string? newVideoUrl) {
      if (string.IsNullOrWhiteSpace(newVideoUrl)) {
        return;
      }
      ValidateVideoUrlFormat(newVideoUrl);
      if (IsPersistent && Session != null && newVideoUrl != oldVideoUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw an InvalidOperationException anyway.
        var duplicate = FindDuplicateVideoUrl(newVideoUrl, Session);
        if (duplicate != null) {
          throw CreateDuplicateVideoUrlUpdateException(duplicate);
        }
      }
    }

    public static void ValidateVideoUrlFormat(string videoUrl) {
      ValidateUrlFormat(videoUrl, nameof(VideoUrl));
    }
  }
}