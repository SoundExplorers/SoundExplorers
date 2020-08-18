using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Piece : RelativeBase {
    private string _audioPath;
    private string _notes;
    private int _pieceNo;
    private string _title;
    private string _videoPath;
    public Piece() : base(typeof(Piece), nameof(PieceNo), typeof(Set)) { }

    [CanBeNull]
    public string AudioPath {
      get => _audioPath;
      set {
        UpdateNonIndexField();
        _audioPath = value;
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
    public string VideoPath {
      get => _videoPath;
      set {
        UpdateNonIndexField();
        _videoPath = value;
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}