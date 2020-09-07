using System;
using System.Collections;
using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a set of Pieces (at least one)
  ///   performed at or in some other way part of an Event.
  /// </summary>
  public class Set : EntityBase {
    private Act _act;
    private Genre _genre;
    private string _notes;
    private int _setNo;

    public Set() : base(typeof(Set), nameof(SetNo), typeof(Event)) {
      Pieces = new SortedChildList<Piece>(this);
      IsPublic = true;
    }

    /// <summary>
    ///   Optionally specifies the Act that played the set.
    /// </summary>
    [CanBeNull]
    public Act Act {
      get => _act;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Act), value);
        _act = value;
      }
    }

    public Event Event {
      get => (Event)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public Genre Genre {
      get => _genre;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Genre), value);
        _genre = value;
      }
    }

    public bool IsPublic { get; set; }

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
        if (value == 0) {
          throw new NoNullAllowedException("SetNo '00' is not valid.");
        }
        UpdateNonIndexField();
        _setNo = value;
        SimpleKey = value.ToString().PadLeft(2, '0');
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Pieces;
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase newParent) {
      if (parentEntityType == typeof(Act)) {
        _act = (Act)newParent;
      } else {
        _genre = (Genre)newParent;
      }
    }
  }
}