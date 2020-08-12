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

    public Event() : base(typeof(Event), nameof(Date)) {
      Sets = new SortedChildList<Set>(this);
    }

    public DateTime Date {
      get => _date;
      set {
        UpdateNonIndexField();
        _date = value;
      }
    }

    [NotNull]
    public Location Location {
      get => _location;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Location), value);
        _location = value;
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
      SessionBase session) {
      throw new NotSupportedException();
    }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }

    protected override RelativeBase GetIdentifyingParent() {
      return Location;
    }

    protected override string GetSimpleKey() {
      return $"{Date:yyyy/MM/dd}";
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