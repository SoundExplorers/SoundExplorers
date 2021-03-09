using System;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a set of Pieces (at least one)
  ///   performed at or in some other way part of an Event.
  /// </summary>
  public class Set : EntityBase {
    private Act? _act;
    private Genre _genre = null!;
    private bool _isPublic;
    private string _notes = null!;
    private int _setNo;

    public Set() : base(typeof(Set), nameof(SetNo), typeof(Event)) {
      Pieces = new SortedChildList<Piece>();
      IsPublic = true;
    }

    /// <summary>
    ///   Optionally specifies the Act that played the set.
    /// </summary>
    public Act Act {
      get => _act!;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Act), value);
        _act = value;
      }
    }

    public Event Event {
      get => (Event)IdentifyingParent!;
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

    public bool IsPublic {
      get => _isPublic;
      set {
        UpdateNonIndexField();
        _isPublic = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public SortedChildList<Piece> Pieces { get; }

    public int SetNo {
      get => _setNo;
      set {
        UpdateNonIndexField();
        _setNo = value;
        SimpleKey = IntegerToSimpleKey(value, nameof(SetNo));
      }
    }

    protected override ISortedChildList GetChildren(Type childType) {
      return Pieces;
    }

    protected override void CheckCanPersist(SessionBase session) {
      if (_act == null) {
        Act = QueryHelper.Read<Act>(Act.DefaultName, session);
      }
      base.CheckCanPersist(session);
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      if (parentEntityType == typeof(Act)) {
        _act = newParent as Act;
      } else {
        _genre = (newParent as Genre)!;
      }
    }
  }
}