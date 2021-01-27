using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an event.
  ///   Usually a performance, but could, for example,
  ///   be a rehearsal.
  /// </summary>
  public class Event : EntityBase {
    private DateTime _date;
    private EventType _eventType = null!;
    private Newsletter? _newsletter;
    private string _notes = null!;
    private Series? _series;

    public Event() : base(typeof(Event), nameof(Date), typeof(Location)) {
      _date = InitialDate;
      Sets = new SortedChildList<Set>();
    }

    public DateTime Date {
      get => _date;
      set {
        if (value <= InitialDate) {
          throw new PropertyConstraintException(
            $"Event Date must be later than {DateToSimpleKey(InitialDate)}.",
            nameof(Date));
        }
        UpdateNonIndexField();
        _date = value.Date;
        SimpleKey = DateToSimpleKey(_date);
      }
    }

    public EventType EventType {
      get => _eventType;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(EventType), value);
        _eventType = value;
      }
    }

    public Location Location {
      get => (Location)IdentifyingParent!;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public Newsletter? Newsletter {
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

    public Series? Series {
      get => _series;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Series), value);
        _series = value;
      }
    }

    public SortedChildList<Set> Sets { get; }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType,
      EntityBase? newParent) {
      if (parentEntityType == typeof(EventType)) {
        _eventType = (newParent as EventType)!;
      } else if (parentEntityType == typeof(Newsletter)) {
        _newsletter = newParent as Newsletter;
      } else {
        _series = newParent as Series;
      }
    }
  }
}