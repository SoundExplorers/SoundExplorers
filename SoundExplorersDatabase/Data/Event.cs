using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Event : RelativeBase {
    private DateTime _date;
    private Newsletter _newsletter;
    private string _notes;
    private Series _series;

    public Event() : base(typeof(Event), nameof(Date), typeof(Location)) {
      Sets = new SortedChildList<Set>(this);
    }

    public DateTime Date {
      get => _date;
      set {
        UpdateNonIndexField();
        _date = value;
        SimpleKey = $"{Date:yyyy/MM/dd}";
      }
    }

    public Location Location {
      get => (Location)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    [CanBeNull]
    public Newsletter Newsletter {
      get => _newsletter;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Newsletter), value);
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
        ChangeNonIdentifyingParent(typeof(Series), value);
        _series = value;
      }
    }

    [NotNull] public SortedChildList<Set> Sets { get; }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }

    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Newsletter)) {
        _newsletter = (Newsletter)newParent;
      } else {
        _series = (Series)newParent;
      }
    }
  }
}