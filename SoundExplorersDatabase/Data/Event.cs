using System;
using System.Collections;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Event : EntityBase {
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

    [CanBeNull]
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
      Type parentEntityType,
      EntityBase newParent) {
      if (parentEntityType == typeof(Newsletter)) {
        _newsletter = (Newsletter)newParent;
      } else {
        _series = (Series)newParent;
      }
    }
  }
}