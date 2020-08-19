using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Piece : EntityBase {
    private Uri _audioUrl;
    private string _notes;
    private int _pieceNo;
    private string _title;
    private Uri _videoUrl;
    public Piece() : base(typeof(Piece), nameof(PieceNo), typeof(Set)) { }

    [CanBeNull]
    public Uri AudioUrl {
      get => _audioUrl;
      set {
        UpdateNonIndexField();
        _audioUrl = value;
      }
    }

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
        UpdateNonIndexField();
        _videoUrl = value;
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}