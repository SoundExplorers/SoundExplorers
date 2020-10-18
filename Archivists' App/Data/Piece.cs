using System;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a piece for which an audio and/or video
  ///   recording is archived.
  /// </summary>
  public class Piece : EntityBase {
    private string _audioUrl;
    private string _notes;
    private int _pieceNo;
    private string _title;
    private string _videoUrl;

    public Piece() : base(typeof(Piece), nameof(PieceNo), typeof(Set)) {
      Credits = new SortedChildList<Credit>(this);
    }

    /// <summary>
    ///   The URL where the audio recording, if available, of the piece is archived.
    ///   Must be unique if specified.
    /// </summary>
    [CanBeNull]
    public string AudioUrl {
      get => _audioUrl;
      set {
        CheckCanChangeAudioUrl(_audioUrl, value);
        UpdateNonIndexField();
        _audioUrl = value;
      }
    }

    [NotNull] public SortedChildList<Credit> Credits { get; }

    [CanBeNull]
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
        if (value == 0) {
          throw new PropertyConstraintException("PieceNo '00' is not valid.",
            nameof(PieceNo));
        }
        UpdateNonIndexField();
        _pieceNo = value;
        SimpleKey = value.ToString().PadLeft(2, '0');
      }
    }

    public Set Set {
      get => (Set)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    [CanBeNull]
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
    [CanBeNull]
    public string VideoUrl {
      get => _videoUrl;
      set {
        CheckCanChangeVideoUrl(_videoUrl, value);
        UpdateNonIndexField();
        _videoUrl = value;
      }
    }

    private void CheckCanChangeAudioUrl([CanBeNull] string oldAudioUrl,
      [CanBeNull] string newAudioUrl) {
      if (string.IsNullOrWhiteSpace(newAudioUrl)) {
        return;
      }
      try {
        var dummy = new Uri(newAudioUrl, UriKind.Absolute);
      } catch (UriFormatException) {
        throw new PropertyConstraintException(
          $"Invalid AudioUrl format '{newAudioUrl}'.", nameof(AudioUrl));
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

    private void CheckCanChangeVideoUrl([CanBeNull] string oldVideoUrl,
      [CanBeNull] string newVideoUrl) {
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
      Piece duplicate;
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
    }

    [CanBeNull]
    private Piece FindDuplicateAudioUrl([NotNull] string audioUrl,
      [NotNull] SessionBase session) {
      return QueryHelper.Find<Piece>(
        piece => piece.AudioUrl != null && piece.AudioUrl.Equals(audioUrl) &&
                 !piece.Oid.Equals(Oid),
        session);
    }

    [CanBeNull]
    private Piece FindDuplicateVideoUrl([NotNull] string videoUrl,
      [NotNull] SessionBase session) {
      return QueryHelper.Find<Piece>(
        piece => piece.VideoUrl != null && piece.VideoUrl.Equals(videoUrl) &&
                 !piece.Oid.Equals(Oid),
        session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Credits;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}