using System;
using System.Collections;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Set : RelativeBase {
    private Act _act;
    private string _notes;
    private int _setNo;

    public Set() : base(typeof(Set), nameof(SetNo), typeof(Event)) {
      Pieces = new SortedChildList<Piece>(this);
    }

    [CanBeNull]
    public Act Act {
      get => _act;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Act), value);
        _act = value;
      }
    }

    [CanBeNull]
    public Event Event {
      get => (Event)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
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

    [NotNull] public SortedChildList<Piece> Pieces { get; }

    public int SetNo {
      get => _setNo;
      set {
        UpdateNonIndexField();
        _setNo = value;
        SimpleKey = value.ToString().PadLeft(2, '0');
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Pieces;
    }

    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      _act = (Act)newParent;
    }
  }
}