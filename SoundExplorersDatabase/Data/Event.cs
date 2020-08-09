using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Event : RelativeBase {
    private DateTime _date;
    private Location _location;
    private Newsletter _newsletter;
    private string _notes;
    private Series _series;

    public Event() : base(typeof(Event)) {
      Sets = new SortedChildList<Set>(this);
    }

    public DateTime Date {
      get => _date;
      set {
        UpdateNonIndexField();
        _date = value;
        SetKey($"{value:yyyy/MM/dd}");
      }
    }
    
    public override RelativeBase IdentifyingParent => Location;

    [NotNull]
    public Location Location {
      get => _location;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Location), value);
        _location = value;
        //SetKey(Date, value);
      }
    }

    [CanBeNull]
    public Newsletter Newsletter {
      get => _newsletter;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Newsletter), value);
        _newsletter = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    [CanBeNull]
    public Series Series {
      get => _series;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Series), value);
        _series = value;
      }
    }
    
    [NotNull] public SortedChildList<Set> Sets { get; }

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey(
      [NotNull] SessionBase session) {
      throw new NotSupportedException();
    }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }

    protected override void OnParentFieldToBeUpdated(Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Location)) {
        _location = (Location)newParent;
      } else if (parentPersistableType == typeof(Newsletter)) {
        _newsletter = (Newsletter)newParent;
      } else {
        _series = (Series)newParent;
      }
    }
  }
}