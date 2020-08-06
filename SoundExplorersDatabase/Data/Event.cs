using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Event : RelativeBase {
    private DateTime _date;
    private Location _location;
    private string _notes;

    public Event() : base(typeof(Event)) { }

    public DateTime Date {
      get => _date;
      set {
        UpdateNonIndexField();
        _date = value;
        SetKey(value, Location);
      }
    }

    [NotNull]
    public Location Location {
      get => _location;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Location), value);
        _location = value;
        SetKey(Date, value);
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey([NotNull] SessionBase session) {
      throw new NotSupportedException();
    }

    protected override IDictionary GetChildren(Type childType) {
      throw new NotImplementedException();
    }

    protected override void OnParentFieldToBeUpdated(Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Location)) {
        _location = (Location)newParent;
      }
    }

    private void SetKey(DateTime date, [CanBeNull] Location location) {
      SetKey($"{date:yyyy/MM/dd} {location?.Name}");
    }
  }
}