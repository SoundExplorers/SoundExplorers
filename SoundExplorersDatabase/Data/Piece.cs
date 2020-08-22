using System;
using System.Collections;
using System.Data;
using System.Data.Linq;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Piece : EntityBase {
    private Uri _audioUrl;
    private string _notes;
    private int _pieceNo;
    private string _title;
    private Uri _videoUrl;

    public Piece() : base(typeof(Piece), nameof(PieceNo), typeof(Set)) {
      Credits = new SortedChildList<Credit>(this);
    }

    [CanBeNull]
    public Uri AudioUrl {
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
          throw new NoNullAllowedException("PieceNo '00' is not valid.");
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

    [CanBeNull]
    public Uri VideoUrl {
      get => _videoUrl;
      set {
        CheckCanChangeVideoUrl(_videoUrl, value);
        UpdateNonIndexField();
        _videoUrl = value;
      }
    }

    private void CheckCanChangeAudioUrl([CanBeNull] Uri oldAudioUrl,
      [CanBeNull] Uri newAudioUrl) {
      if (newAudioUrl == null) {
        return;
      }
      if (IsPersistent && Session != null && newAudioUrl != oldAudioUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateAudioUrl(newAudioUrl, Session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"The Audio URL of Piece '{Key}' cannot be changed to " +
            $"'{newAudioUrl}'. Piece '{duplicate.Key}' " +
            "has already been persisted with that Audio URL.");
        }
      }
    }

    private void CheckCanChangeVideoUrl([CanBeNull] Uri oldVideoUrl,
      [CanBeNull] Uri newVideoUrl) {
      if (newVideoUrl == null) {
        return;
      }
      if (IsPersistent && Session != null && newVideoUrl != oldVideoUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateVideoUrl(newVideoUrl, Session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"The Video URL of Piece '{Key}' cannot be changed to " +
            $"'{newVideoUrl}'. Piece '{duplicate.Key}' " +
            "has already been persisted with that Video URL.");
        }
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      Piece duplicate;
      if (AudioUrl != null) {
        duplicate = FindDuplicateAudioUrl(AudioUrl, session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"Piece '{Key}' " +
            "cannot be persisted because Piece " +
            $"'{duplicate.Key}' " +
            $"has already been persisted with the same Audio URL '{AudioUrl}'.");
        }
      }
      if (VideoUrl != null) {
        duplicate = FindDuplicateVideoUrl(VideoUrl, session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"Piece '{Key}' " +
            "cannot be persisted because Piece " +
            $"'{duplicate.Key}' " +
            $"has already been persisted with the same Video URL '{VideoUrl}'.");
        }
      }
    }

    [CanBeNull]
    private Piece FindDuplicateAudioUrl([NotNull] Uri audioUrl,
      [NotNull] SessionBase session) {
      return QueryHelper.Find<Piece>(
        piece => piece.AudioUrl != null && piece.AudioUrl.Equals(audioUrl) &&
                 !piece.Oid.Equals(Oid),
        session);
    }

    [CanBeNull]
    private Piece FindDuplicateVideoUrl([NotNull] Uri videoUrl,
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
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}