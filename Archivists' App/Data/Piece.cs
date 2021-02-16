using System;
using System.Collections;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a piece for which an audio and/or video
  ///   recording is archived.
  /// </summary>
  public class Piece : EntityBase {
    private string _audioUrl = null!;
    private TimeSpan _duration;
    private string _notes = null!;
    private int _pieceNo;
    private string _title = null!;
    private string _videoUrl = null!;

    public Piece() : base(typeof(Piece), nameof(PieceNo), typeof(Set)) {
      Credits = new SortedChildList<Credit>();
    }

    /// <summary>
    ///   The URL where the audio recording, if available, of the piece is archived.
    ///   Must be unique if specified.
    /// </summary>
    public string AudioUrl {
      get => _audioUrl;
      set {
        CheckCanChangeAudioUrl(_audioUrl, value);
        UpdateNonIndexField();
        _audioUrl = value;
      }
    }

    public SortedChildList<Credit> Credits { get; }

    public TimeSpan Duration {
      get => _duration;
      set {
        ValidateDuration(value);
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
        CheckCanChangeVideoUrl(_videoUrl, value);
        UpdateNonIndexField();
        _videoUrl = value;
      }
    }

    private void CheckCanChangeAudioUrl(string? oldAudioUrl,
      string? newAudioUrl) {
      if (string.IsNullOrWhiteSpace(newAudioUrl)) {
        return;
      }
      try {
        var dummy = new Uri(newAudioUrl, UriKind.Absolute);
      } catch (UriFormatException) {
        throw new PropertyConstraintException(
          $"Invalid AudioUrl format: '{newAudioUrl}'.", nameof(AudioUrl));
      }
      if (IsPersistent && Session != null && newAudioUrl != oldAudioUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateAudioUrl(newAudioUrl, Session);
        if (duplicate != null) {
          throw new PropertyConstraintException(
            "Audio URL cannot be set to " +
            $"'{newAudioUrl}'. Piece '{duplicate.Key}' " +
            "already exists with that Audio URL.", nameof(AudioUrl));
        }
      }
    }

    private void CheckCanChangeVideoUrl(string? oldVideoUrl, string? newVideoUrl) {
      if (string.IsNullOrWhiteSpace(newVideoUrl)) {
        return;
      }
      try {
        var dummy = new Uri(newVideoUrl, UriKind.Absolute);
      } catch (UriFormatException) {
        throw new PropertyConstraintException(
          $"Invalid VideoUrl format: '{newVideoUrl}'.", nameof(VideoUrl));
      }
      if (IsPersistent && Session != null && newVideoUrl != oldVideoUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateVideoUrl(newVideoUrl, Session);
        if (duplicate != null) {
          throw new PropertyConstraintException(
            "Video URL cannot be set to " +
            $"'{newVideoUrl}'. Piece '{duplicate.Key}' " +
            "already exists with that Video URL.", nameof(VideoUrl));
        }
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      Piece? duplicate;
      if (!string.IsNullOrWhiteSpace(AudioUrl)) {
        duplicate = FindDuplicateAudioUrl(AudioUrl, session);
        if (duplicate != null) {
          throw new PropertyConstraintException(
            "Piece cannot be added because Piece " +
            $"'{duplicate.Key}' " +
            $"already exists with the same Audio URL '{AudioUrl}'.", nameof(AudioUrl));
        }
      }
      if (!string.IsNullOrWhiteSpace(VideoUrl)) {
        duplicate = FindDuplicateVideoUrl(VideoUrl, session);
        if (duplicate != null) {
          throw new PropertyConstraintException(
            "Piece cannot be added because Piece " +
            $"'{duplicate.Key}' " +
            $"already exists with the same Video URL '{VideoUrl}'.", nameof(VideoUrl));
        }
      }
      if (Duration == TimeSpan.Zero) {
        throw new PropertyConstraintException(
          "Piece cannot be added because its Duration has not been specified.", 
          nameof(Duration));
      }
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

    protected override IDictionary GetChildren(Type childType) {
      return Credits;
    }
 
    private static void ValidateDuration(TimeSpan value) {
      if (value < TimeSpan.FromSeconds(1) || value >= TimeSpan.FromHours(10)) {
        throw new PropertyValueOutOfRangeException(
          "Duration must be between 1 second and 9 hours, 59 minutes, 59 seconds.", 
          nameof(Duration));
      }
    }
  }
}